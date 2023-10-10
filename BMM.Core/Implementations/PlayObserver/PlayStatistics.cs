using System.Globalization;
using System.Runtime.CompilerServices;
using BMM.Api.Abstraction;
using BMM.Api.Framework;
using BMM.Api.Implementation.Clients.Contracts;
using BMM.Api.Implementation.Models;
using BMM.Core.Constants;
using BMM.Core.Implementations.Analytics;
using BMM.Core.Implementations.Exceptions;
using BMM.Core.Implementations.FirebaseRemoteConfig;
using BMM.Core.Implementations.Player.Interfaces;
using BMM.Core.Implementations.PlayObserver.Model;
using BMM.Core.Implementations.Security;
using BMM.Core.Messages;
using BMM.Core.Messages.MediaPlayer;
using BMM.Core.NewMediaPlayer.Constants;
using MvvmCross.Plugin.Messenger;

namespace BMM.Core.Implementations.PlayObserver
{
    /// <summary>
    /// Tracks what songs the user listens to and logs extensive information about each track (e.g. percentage listened and total listen time).
    ///
    /// Edge cases:
    /// - Listen the second half of a track, have RepeatMode = One and listens to the first half of the track.
    ///   Then we will have 2 logs of ~50% which will not be counted as a play in Azure Portal(where we consider a play if the track is listened more than 60%)
    /// </summary>
    public class PlayStatistics : IPlayStatistics
    {
        public const double TimeCompareToleranceInMillis = 0.1;
        private readonly IAnalytics _analytics;
        private readonly IUserStorage _userStorage;
        private readonly ILogger _logger;
        private readonly IMeasurementCalculator _measurementCalculator;
        private readonly IStatisticsClient _client;
        private readonly IExceptionHandler _exceptionHandler;
        private readonly IMvxMessenger _messenger;
        private readonly IFirebaseRemoteConfig _config;
        private readonly IPlaybackHistoryService _playbackHistoryService;

        public double StartOfNextPortion { get; private set; }
        public DateTime StartTimeOfNextPortion { get; private set; }

        private const string Tag = "PlayStatistics";

        public ITrackModel CurrentTrack { get; private set; }
        public IList<IMediaTrack> CurrentQueue { get; private set; }
        public bool IsCurrentQueueSaved { get; set; }
        public bool IsPlaying { get; private set; }
        public decimal DesiredPlaybackRate { get; private set; } = PlayerConstants.NormalPlaybackSpeed;

        /// <summary>
        /// We want to be able to extend <see cref="Clear"/> in <see cref="PlayStatisticsDecorator"/>. Therefore we use this Action as work-around to pass the command through all Decorator layers.
        /// </summary>
        public Action TriggerClear { get; set; }

        public IList<ListenedPortion> PortionsListened { get; } = new List<ListenedPortion>();

        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        public PlayStatistics(
            IAnalytics analytics,
            IUserStorage userStorage,
            ILogger logger,
            IMeasurementCalculator measurementCalculator,
            IStatisticsClient client,
            IExceptionHandler exceptionHandler,
            IMvxMessenger messenger,
            IFirebaseRemoteConfig config,
            IPlaybackHistoryService playbackHistoryService)
        {
            _analytics = analytics;
            _userStorage = userStorage;
            _logger = logger;
            _measurementCalculator = measurementCalculator;
            _client = client;
            _exceptionHandler = exceptionHandler;
            _messenger = messenger;
            _config = config;
            _playbackHistoryService = playbackHistoryService;
        }

        public void OnPlaybackStateChanged(IPlaybackState state)
        {
            // We add a portion when the user pauses the playback or changes the playback rate.
            if (!state.IsPlaying && IsPlaying || DesiredPlaybackRate != state.DesiredPlaybackRate)
            {
                AddPortionListened(state.CurrentPosition, DesiredPlaybackRate);
            }
            else if (state.IsPlaying && !IsPlaying)
            {
                // This is mainly needed for livestream where the current position changes even though you're not listening
                StartNextPortion(state.CurrentPosition);
            }

            IsPlaying = state.IsPlaying;
            DesiredPlaybackRate = state.DesiredPlaybackRate;
        }
        
        public void OnCurrentQueueChanged(CurrentQueueChangedMessage message)
        {
            CurrentQueue = message.Queue;
            IsCurrentQueueSaved = false;
        }

        public void OnSeeked(double currentPosition, double seekedPosition)
        {
            if (CurrentTrack == null
                || StartOfNextPortion > currentPosition
                || Math.Abs(currentPosition - seekedPosition) < TimeCompareToleranceInMillis)
                return;

            AddPortionListened(currentPosition, DesiredPlaybackRate);
            StartNextPortion(seekedPosition);
        }

        public void OnCurrentTrackChanged(CurrentTrackChangedMessage message)
        {
            _exceptionHandler.FireAndForgetWithoutUserMessages(async () =>
            {
                if (CurrentTrack != null && message.CurrentTrack != null)
                {
                    var elapsedTimeSinceLastPortion = DateTime.UtcNow - StartTimeOfNextPortion;
                    if (IsPlaying && elapsedTimeSinceLastPortion.Seconds > 5)
                    {
                        _analytics.LogEvent("probably didn't finish a portion",
                            new Dictionary<string, object>
                            {
                                {"TrackId", CurrentTrack.Id}, {"Portions", PortionsListened.Count}, {"StartOfNextPortion", StartOfNextPortion},
                                {"ElapsedSeconds", elapsedTimeSinceLastPortion.Seconds}
                            });
                    }

                    await LogPlayedTrack();
                    StartNextPortion(message.StartingPositionMs);
                }

                if (message.CurrentTrack != null)
                    CurrentTrack = message.CurrentTrack;
            });
        }

        public void OnTrackCompleted(TrackCompletedMessage message)
        {
            _exceptionHandler.FireAndForgetWithoutUserMessages(async () =>
            {
                if (CurrentTrack != null)
                {
                    AddPortionListened(CurrentTrack.Duration, DesiredPlaybackRate);
                    await LogPlayedTrack();
                    StartNextPortion();
                }
            });
        }

        public void AddPortionListened(double currentPosition, decimal playbackRate)
        {
            if (Math.Abs(StartOfNextPortion - currentPosition) < TimeCompareToleranceInMillis)
                return;

            if (_config.UseExtendedStreakLogging)
            {
                _analytics.LogEvent("PlayStatisticsEvents|Add portion listened", new Dictionary<string, object>
                        {{"Track", CurrentTrack?.Id}, {"Portions", PortionsListened.Count}, {"StartOfNewPortion", StartOfNextPortion}, {"EndOfNewPortion", currentPosition}});
            }

            PortionsListened.Add(new ListenedPortion
            {
                Start = StartOfNextPortion,
                StartTime = StartTimeOfNextPortion,
                End = currentPosition,
                EndTime = DateTime.UtcNow,
                PlaybackRate = playbackRate
            });
            StartNextPortion(currentPosition);
        }

        public PlayMeasurements GetMeasurementForNewPosition(long position)
        {
            var list = PortionsListened.Clone();
            if (Math.Abs(StartOfNextPortion - position) > TimeCompareToleranceInMillis)
            {
                list.Add(new ListenedPortion
                {
                    Start = StartOfNextPortion,
                    StartTime = StartTimeOfNextPortion,
                    End = position,
                    EndTime = DateTime.UtcNow,
                    PlaybackRate = DesiredPlaybackRate
                });
            }

            return _measurementCalculator.Calculate(CurrentTrack.Duration, list);
        }

        public async Task TrySendSavedStreakPointsEvents()
        {
            await _client.PostStreakPoints(Enumerable.Empty<StreakPointEvent>().ToList());
        }

        public async Task PostStreakPoints(ITrackModel track, PlayMeasurements measurements)
        {
            var user = _userStorage.GetUser();

            await _client.PostStreakPoints(new List<StreakPointEvent>
            {
                new()
                {
                    PersonId = user.PersonId,
                    TrackId = track.Id,
                    TimestampStart = measurements.TimestampStart,
                    Language = track.Language,
                    PlaybackOrigin = track.PlaybackOrigin,
                    LastPosition = measurements.LastPosition,
                    AdjustedPlaybackSpeed = measurements.AdjustedPlaybackSpeed,
                    OS = OperatingSystem.IsIOS()
                        ? "ios"
                        : "android"
                }
            });
        }
        
        public async Task PostListeningEvent(ITrackModel track, PlayMeasurements measurements)
        {
            var user = _userStorage.GetUser();

            await _client.PostListeningEvents(new List<ListeningEvent>
            {
                new()
                {
                    PersonId = user.PersonId,
                    TrackId = track.Id,
                    TimestampStart = measurements.TimestampStart,
                    Language = track.Language,
                    PlaybackOrigin = track.PlaybackOrigin,
                    LastPosition = measurements.LastPosition,
                    AdjustedPlaybackSpeed = measurements.AdjustedPlaybackSpeed,
                    OS = OperatingSystem.IsIOS()
                        ? "ios"
                        : "android"
                }
            });
        }

        public void OnCurrentTrackWillChange(double currentPosition, decimal playbackRate)
        {
            AddPortionListened(currentPosition, playbackRate);
        }

        private async Task LogPlayedTrack()
        {
            await _semaphore.WaitAsync();
            try
            {
                if (CurrentTrack == null || !PortionsListened.Any())
                {
                    _analytics.LogEvent("probably prevented duplicate Track played", new Dictionary<string, object> {{"Track", CurrentTrack?.Id}});
                    return;
                }

                var measurements = _measurementCalculator.Calculate(CurrentTrack.Duration, PortionsListened);
                if (measurements == null)
                {
                    _logger.Info(Tag, "Clear portions since measurements are null");
                    TriggerClear();
                    return;
                }

                if (CurrentTrack.Tags?.Contains(PodcastsConstants.FromKaareTagName) == true ||
                    CurrentTrack.Tags?.Contains(PodcastsConstants.BibleStudyTagName) == true)
                    _messenger.Publish(new StreakTrackCompletedMessage(this) {Track = CurrentTrack, Measurements = measurements});

                LogListenedPortionsIfUniqueSecondsListenedAreGreaterThanSpentTime(measurements.UniqueSecondsListened, measurements.SpentTime);

                var ev = ComposeEvent(measurements);
                await _playbackHistoryService.AddPlayedTrack((Track)CurrentTrack, ev.LastPosition, ev.TimestampStart);

                await WriteEvent(ev);
                TriggerClear();
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public void Clear()
        {
            if (_config.UseExtendedStreakLogging)
                _analytics.LogEvent("PlayStatisticsEvents|clear portions listened");
            PortionsListened.Clear();
        }

        public TrackPlayedEvent ComposeEvent(PlayMeasurements measurements, [CallerMemberName] string callerName = "")
        {
            var user = _userStorage.GetUser();

            var ev = new TrackPlayedEvent
            {
                Id = Guid.NewGuid(),
                PersonId = user.PersonId,
                AnalyticsId = user.AnalyticsId,
                TrackId = CurrentTrack.Id,
                Percentage = measurements.Percentage,
                SpentTime = measurements.SpentTime,
                TimestampStart = measurements.TimestampStart,
                TimestampEnd = measurements.TimestampEnd,
                UniqueSecondsListened = measurements.UniqueSecondsListened,
                Status = measurements.Status,
                TrackLength = measurements.TrackLength,
                TypeOfTrack = CurrentTrack.Subtype,
                Availability = CurrentTrack.Availability,
                AlbumId = CurrentTrack.ParentId,
                Tags = CurrentTrack.Tags,
                Language = CurrentTrack.Language,
                PlaybackOrigin = CurrentTrack.PlaybackOrigin,
                LastPosition = measurements.LastPosition,
                Track = (Track) CurrentTrack,
                AdjustedPlaybackSpeed = measurements.AdjustedPlaybackSpeed,
                OS = OperatingSystem.IsIOS()
                    ? "ios"
                    : "android"
            };

            LogMissingAnalyticsIdIfNeeded(ev, nameof(ComposeEvent), callerName);
            return ev;
        }

        public async Task WriteEvent(TrackPlayedEvent ev, [CallerMemberName] string callerName = "")
        {
            var dict = new Dictionary<string, object>
            {
                {"trackId", ev.TrackId},
                {"percentage", ev.Percentage},
                {"spentTime", ev.SpentTime},
                {"timestampStart", ev.TimestampStart},
                {"timestampEnd", ev.TimestampEnd},
                {"uniqueSecondsListened", ev.UniqueSecondsListened},
                {"trackLength", ev.TrackLength},
                {"typeOfTrack", ev.TypeOfTrack},
                {"availability", ev.Availability},
                {"albumId", ev.AlbumId},
                {"tags", string.Join(",", ev.Tags)},
                {"language", ev.Language},
                {"sentAfterStartup", ev.SentAfterStartup},
                {"playbackOrigin", ev.PlaybackOrigin},
                {"lastPosition", ev.LastPosition},
                {"adjustedPlaybackSpeed", ev.AdjustedPlaybackSpeed},
                {nameof(User.AnalyticsId), ev.AnalyticsId}
            }; // Since an event can only have 20 properties, we can't add more than this.

            LogMissingAnalyticsIdIfNeeded(ev, nameof(WriteEvent), callerName);
            
            _analytics.LogEvent("Track played", dict);
            await _client.PostTrackPlayedEvent(new[] {ev});
        }

        private void LogMissingAnalyticsIdIfNeeded(TrackPlayedEvent ev, string method, string callerName)
        {
            if (string.IsNullOrEmpty(ev.AnalyticsId))
            {
                _analytics.LogEvent("Missing AnalyticsId",
                    new Dictionary<string, object>()
                    {
                        { "Method", method },
                        { "Caller", callerName }
                    });
            }
        }

        private void StartNextPortion(double startOfNextPortion = 0d)
        {
            StartOfNextPortion = startOfNextPortion;
            StartTimeOfNextPortion = DateTime.UtcNow;
        }

        private void LogListenedPortionsIfUniqueSecondsListenedAreGreaterThanSpentTime(double uniqueSecondsListened, double spentTime)
        {
            if (uniqueSecondsListened > spentTime)
            {
                var portions = ConvertPortionsToReadableFormat(PortionsListened);
                LogListenedPortionsWithExtras(portions, uniqueSecondsListened, spentTime);
            }
        }

        private List<string> ConvertPortionsToReadableFormat(IList<ListenedPortion> listenedPortions)
        {
            var portions = new List<string>();
            foreach(var portion in listenedPortions)
            {
                var preparedPortion = "{"
                    + TimeSpan.FromMilliseconds(portion.Start).TotalSeconds.ToString(CultureInfo.InvariantCulture) + ", "
                    + TimeSpan.FromMilliseconds(portion.End).TotalSeconds.ToString(CultureInfo.InvariantCulture) + "}";

                portions.Add(preparedPortion);
            }
            return portions;
        }

        private void LogListenedPortionsWithExtras(List<string> portions, double uniqueSecondsListened, double spentTime)
        {
            _analytics.LogEvent("Log listened portions",
            new Dictionary<string, object>
            {
                { "Portions", string.Join(", ", portions.ToArray()) },
                /* We log UniqueSecondsListened and SpentTime too
                 * in order to have better filtering possibilities while querying. */
                { "UniqueSecondsListened", uniqueSecondsListened.ToString(CultureInfo.InvariantCulture) },
                { "SpentTime", spentTime.ToString(CultureInfo.InvariantCulture) }
            });
        }
    }
}