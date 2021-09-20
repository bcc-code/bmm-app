using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BMM.Api.Abstraction;
using BMM.Api.Framework;
using BMM.Api.Implementation.Clients.Contracts;
using BMM.Api.Implementation.Models;
using BMM.Core.Implementations.Analytics;
using BMM.Core.Implementations.Exceptions;
using BMM.Core.Implementations.FirebaseRemoteConfig;
using BMM.Core.Implementations.PlayObserver.Model;
using BMM.Core.Implementations.Security;
using BMM.Core.Messages;
using BMM.Core.Messages.MediaPlayer;
using BMM.Core.ViewModels;
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
        private readonly IAnalytics _analytics;
        private readonly IUserStorage _userStorage;
        private readonly ILogger _logger;
        private readonly IMeasurementCalculator _measurementCalculator;
        private readonly IStatisticsClient _client;
        private readonly IExceptionHandler _exceptionHandler;
        private readonly IMvxMessenger _messenger;
        private readonly IFirebaseRemoteConfig _config;

        public double StartOfNextPortion { get; private set; }
        public DateTime StartTimeOfNextPortion { get; private set; }

        private const string Tag = "PlayStatistics";

        public ITrackModel CurrentTrack { get; private set; }
        public bool IsPlaying { get; private set; }

        /// <summary>
        /// We want to be able to extend <see cref="Clear"/> in <see cref="PlayStatisticsDecorator"/>. Therefore we use this Action as work-around to pass the command through all Decorator layers.
        /// </summary>
        public Action TriggerClear { get; set; }

        public IList<ListenedPortion> PortionsListened { get; } = new List<ListenedPortion>();

        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        public PlayStatistics(IAnalytics analytics, IUserStorage userStorage, ILogger logger, IMeasurementCalculator measurementCalculator, IStatisticsClient client,
            IExceptionHandler exceptionHandler, IMvxMessenger messenger, IFirebaseRemoteConfig config)
        {
            _analytics = analytics;
            _userStorage = userStorage;
            _logger = logger;
            _measurementCalculator = measurementCalculator;
            _client = client;
            _exceptionHandler = exceptionHandler;
            _messenger = messenger;
            _config = config;
        }

        public void OnPlaybackStateChanged(IPlaybackState state)
        {
            // We add a portion when the user pauses the playback.
            if (!state.IsPlaying && IsPlaying)
            {
                AddPortionListened(state.CurrentPosition);
            }
            else if (state.IsPlaying && !IsPlaying)
            {
                // This is mainly needed for livestream where the current position changes even though you're not listening
                StartNextPortion(state.CurrentPosition);
            }

            IsPlaying = state.IsPlaying;
        }

        public void OnSeeked(double currentPosition, double seekedPosition)
        {
            if (CurrentTrack == null)
                return;

            AddPortionListened(currentPosition);
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
                    StartNextPortion();
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
                    AddPortionListened(CurrentTrack.Duration);
                    await LogPlayedTrack();
                    StartNextPortion();
                }
            });
        }

        public void AddPortionListened(double currentPosition)
        {
            if (StartOfNextPortion == currentPosition)
                return;

            if (_config.UseExtendedStreakLogging)
            {
                _analytics.LogEvent("PlayStatisticsEvents|Add portion listened", new Dictionary<string, object>
                        {{"Track", CurrentTrack?.Id}, {"Portions", PortionsListened.Count}, {"StartOfNewPortion", StartOfNextPortion}, {"EndOfNewPortion", currentPosition}});
            }

            PortionsListened.Add(new ListenedPortion {Start = StartOfNextPortion, StartTime = StartTimeOfNextPortion, End = currentPosition, EndTime = DateTime.UtcNow});
            StartNextPortion(currentPosition);
        }

        public PlayMeasurements GetMeasurementForNewPosition(long position)
        {
            var list = PortionsListened.Clone();
            if (StartOfNextPortion != position)
                list.Add(new ListenedPortion {Start = StartOfNextPortion, StartTime = StartTimeOfNextPortion, End = position, EndTime = DateTime.UtcNow});

            return _measurementCalculator.Calculate(CurrentTrack.Duration, list);
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

                if (CurrentTrack.Tags?.Contains(FraKaareTeaserViewModel.FromKaareTagName) == true)
                    _messenger.Publish(new FraKaareTrackCompletedMessage(this) {Track = CurrentTrack, Measurements = measurements});

                LogListenedPortionsIfUniqueSecondsListenedAreGreaterThanSpentTime(measurements.UniqueSecondsListened, measurements.SpentTime);

                var ev = ComposeEvent(measurements);
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

        public TrackPlayedEvent ComposeEvent(PlayMeasurements measurements)
        {
            var user = _userStorage.GetUser();
            return new TrackPlayedEvent
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
                LastPosition = measurements.LastPosition
            };
        }

        public async Task WriteEvent(TrackPlayedEvent ev)
        {
            var dict = new Dictionary<string, object>
            {
                {"trackId", ev.TrackId},
                {"percentage", ev.Percentage},
                {"spentTime", ev.SpentTime},
                {"timestampStart", ev.TimestampStart},
                {"timestampEnd", ev.TimestampEnd},
                {"uniqueSecondsListened", ev.UniqueSecondsListened},
                {"statusListened", ev.Status},
                {"trackLength", ev.TrackLength},
                {"typeOfTrack", ev.TypeOfTrack},
                {"availability", ev.Availability},
                {"albumId", ev.AlbumId},
                {"tags", string.Join(",", ev.Tags)},
                {"language", ev.Language},
                {"sentAfterStartup", ev.SentAfterStartup},
                {"playbackOrigin", ev.PlaybackOrigin},
                {"lastPosition", ev.LastPosition}
            };

            if (_config.UseAnalyticsId)
                dict.Add(nameof(User.AnalyticsId), ev.AnalyticsId);
            else
                dict.Add(nameof(User.PersonId), ev.PersonId);

            _analytics.LogEvent("Track played", dict);

            await _client.PostTrackPlayedEvent(new[] {ev});
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