using System.Timers;
using BMM.Api.Abstraction;
using BMM.Api.Implementation.Models;
using BMM.Core.Extensions;
using BMM.Core.Implementations.Analytics;
using BMM.Core.Implementations.Exceptions;
using BMM.Core.Implementations.PlayObserver.Model;
using BMM.Core.Implementations.Storage;
using BMM.Core.Messages;
using BMM.Core.Messages.MediaPlayer;
using BMM.Core.NewMediaPlayer.Abstractions;
using MvvmCross.Plugin.Messenger;

namespace BMM.Core.Implementations.PlayObserver.Streak
{
    public interface IStreakObserver
    {
        Task UpdateStreakIfLocalVersionIsNewer(List<Document> documents);
    }

    /// <summary>
    /// Detects if a user is playing today's episode of Fra Kaare and updates the <see cref="ListeningStreak"/> without waiting for the information to be sent to the server.
    /// Edge case: It does not work if you start playing without looking at the home screen at some point
    /// </summary>
    public class StreakObserver : IStreakObserver
    {
        private readonly IMediaPlayer _mediaPlayer;
        private readonly PlayObserverOrchestrator _playObserver;
        private readonly IMvxMessenger _messenger;
        private readonly IExceptionHandler _exceptionHandler;
        private readonly IAnalytics _analytics;
        private readonly IPlayStatistics _playStatistics;
        private ListeningStreak _latestStreak;

        private readonly MvxSubscriptionToken _trackCompletedToken;
        private readonly MvxSubscriptionToken _trackChangedToken;

        /// <summary>
        /// timer to check every few seconds <see cref="RetryIntervalInSeconds"/> if enough has been listened to update the streak.
        /// It runs the first time, when it's theoretically possible to have listened to <see cref="MinListeningPercentage"/> of today's episode.
        /// </summary>
        private System.Timers.Timer _minListeningTimer;

        private const int MinListeningPercentage = 50;
        private const int RetryIntervalInSeconds = 10;

        public StreakObserver(IMvxMessenger messenger,
            IMediaPlayer mediaPlayer,
            PlayObserverOrchestrator playObserver,
            IExceptionHandler exceptionHandler,
            IAnalytics analytics,
            IPlayStatistics playStatistics)
        {
            _mediaPlayer = mediaPlayer;
            _playObserver = playObserver;
            _exceptionHandler = exceptionHandler;
            _analytics = analytics;
            _playStatistics = playStatistics;
            _messenger = messenger;
            _trackCompletedToken = messenger.Subscribe<StreakTrackCompletedMessage>(TrackCompleted);
            _trackChangedToken = messenger.Subscribe<CurrentTrackChangedMessage>(TrackChanged);
        }

        private void TrackChanged(CurrentTrackChangedMessage message)
        {
            if (_latestStreak == null
                || message.CurrentTrack == null
                || message.CurrentTrack.Id != _latestStreak.TodaysFraKaareTrackId
                || DateTime.UtcNow >= _latestStreak.EligibleUntil.ToUniversalTime()
                || _latestStreak.IsTodayAlreadyListened())
            {
                DisposeTimer();
                return;
            }

            _minListeningTimer ??= new System.Timers.Timer(RetryIntervalInSeconds.ToMilliseconds());
            _minListeningTimer.Elapsed -= CheckIfListenedEnoughYet;
            _minListeningTimer.Elapsed += CheckIfListenedEnoughYet;
            _minListeningTimer.Start();
        }

        private void DisposeTimer()
        {
            if (_minListeningTimer == null)
                return;

            _minListeningTimer.Elapsed -= CheckIfListenedEnoughYet;
            _minListeningTimer.Dispose();
            _minListeningTimer = null;
        }

        private void TrackCompleted(StreakTrackCompletedMessage msg)
        {
            _exceptionHandler.FireAndForgetWithoutUserMessages(async () =>
            {
                await UpdateStreakIfListened(msg.Track, () => msg.Measurements);
            });
        }

        private void CheckIfListenedEnoughYet(object sender, ElapsedEventArgs e)
        {
            if (!_mediaPlayer.IsPlaying)
                return;
            
            _exceptionHandler.FireAndForgetWithoutUserMessages(async () =>
            {
                var updatedStreak = await UpdateStreakIfListened(_mediaPlayer.CurrentTrack,
                    () =>
                    {
                        var position = _mediaPlayer.PlaybackState.CurrentPosition;
                        return _playObserver.GetMeasurementForNewPosition(position);
                    });
                
                if (updatedStreak)
                    DisposeTimer();
            });
        }

        private async Task<bool> UpdateStreakIfListened(ITrackModel track, Func<PlayMeasurements> measurementsFactory)
        {
            if (_latestStreak != null
                && track?.Id == _latestStreak.TodaysFraKaareTrackId
                && !_latestStreak.IsTodayAlreadyListened()
                && DateTime.UtcNow < _latestStreak.EligibleUntil.ToUniversalTime())
            {
                var measurements = measurementsFactory.Invoke();
                if (measurements == null)
                {
                    _analytics.LogEvent("Measurements are null");
                    return false;
                }
                if (measurements.Percentage > MinListeningPercentage)
                {
                    await _playStatistics.PostStreakPoints(track, measurements);
                    
                    _latestStreak.MarkTodayAsListened();
                    _analytics.LogEvent("mark today as listened",
                        new Dictionary<string, object>
                        {
                            {"trackId", track.Id},
                            {"duration", track.Duration},
                            {"currentSecondsListened", measurements.UniqueSecondsListened},
                            {"playbackStarted", measurements.TimestampStart},
                            {"playbackEnded", measurements.TimestampEnd}
                        });
                    _messenger.Publish(new ListeningStreakChangedMessage(this) {ListeningStreak = _latestStreak});
                    await Store(_latestStreak);
                    return true;
                }
            }

            return false;
        }

        public async Task UpdateStreakIfLocalVersionIsNewer(List<Document> documents)
        {
            int index = documents.FindIndex(document => document.DocumentType == DocumentType.ListeningStreak);
            if (index >= 0)
            {
                var streakFromServer = (ListeningStreak)documents[index];
                var localStreak = await GetStoredStreak();
                if (localStreak != null && localStreak.LastChanged.ToUniversalTime() > streakFromServer.LastChanged.ToUniversalTime() &&
                    localStreak.WeekOfTheYear == streakFromServer.WeekOfTheYear)
                {
                    // keep some information from the server to allow offline update of the next episode as well.
                    localStreak.DayOfTheWeek = streakFromServer.DayOfTheWeek;
                    localStreak.TodaysFraKaareTrackId = streakFromServer.TodaysFraKaareTrackId;
                    localStreak.EligibleUntil = streakFromServer.EligibleUntil;

                    documents[index] = localStreak;
                }
                else
                {
                    if (localStreak != null && localStreak.WeekOfTheYear == streakFromServer.WeekOfTheYear)
                    {
                        if (localStreak.Monday == true && streakFromServer.Monday != true
                            || localStreak.Tuesday == true && streakFromServer.Tuesday != true
                            || localStreak.Wednesday == true && streakFromServer.Wednesday != true
                            || localStreak.Thursday == true && streakFromServer.Thursday != true
                            || localStreak.Friday == true && streakFromServer.Friday != true)
                        {
                            _analytics.LogEvent("streak point lost",
                                new Dictionary<string, object>
                                {
                                    {"Week", streakFromServer.WeekOfTheYear}, {"localStreak", localStreak.ToText()},
                                    {"streakFromServer", streakFromServer.ToText()},
                                    {"localLastChanged", localStreak.LastChanged}, {"serverLastChanged", streakFromServer.LastChanged}
                                });
                        }
                    }

                    _latestStreak = streakFromServer;
                    await Store(null);
                }
            }
        }

        private async Task<ListeningStreak> GetStoredStreak() => AppSettings.LatestListeningStreak;
        private async Task Store(ListeningStreak streak) => AppSettings.LatestListeningStreak = streak;
    }
}