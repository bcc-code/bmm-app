using System.Timers;
using BMM.Api.Abstraction;
using BMM.Core.Extensions;
using BMM.Core.Implementations.Analytics;
using BMM.Core.Implementations.Exceptions;
using BMM.Core.Implementations.PlayObserver.Model;
using BMM.Core.Messages.MediaPlayer;
using BMM.Core.NewMediaPlayer.Abstractions;
using MvvmCross.Plugin.Messenger;

namespace BMM.Core.Implementations.PlayObserver.Streak;

public interface IListeningObserver {}

public class ListeningObserver : IListeningObserver
{
    private readonly IMediaPlayer _mediaPlayer;
    private readonly PlayObserverOrchestrator _playObserver;
    private readonly IMvxMessenger _messenger;
    private readonly IExceptionHandler _exceptionHandler;
    private readonly IAnalytics _analytics;
    private readonly IPlayStatistics _playStatistics;

    private readonly MvxSubscriptionToken _trackCompletedToken;
    private readonly MvxSubscriptionToken _trackChangedToken;

    /// <summary>
    /// timer to check every few seconds <see cref="RetryIntervalInSeconds"/> if enough has been listened to of the current track.
    /// </summary>
    private System.Timers.Timer _minListeningTimer;

    private const int MinListeningPercentage = 50;
    private const int RetryIntervalInSeconds = 10;

    public ListeningObserver(IMvxMessenger messenger,
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
        //_trackCompletedToken = messenger.Subscribe<TrackCompletedMessage>(TrackCompleted);
        _trackChangedToken = messenger.Subscribe<CurrentTrackChangedMessage>(TrackChanged);
    }

    private void TrackChanged(CurrentTrackChangedMessage message)
    {
        if (message.CurrentTrack == null)
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

    // private void TrackCompleted(TrackCompletedMessage msg)
    // {
    //     _exceptionHandler.FireAndForgetWithoutUserMessages(async () =>
    //     {
    //         await SendEventIfListened(msg.Track, () => msg.Measurements);
    //     });
    // }

    private void CheckIfListenedEnoughYet(object sender, ElapsedEventArgs e)
    {
        if (!_mediaPlayer.IsPlaying)
            return;
            
        _exceptionHandler.FireAndForgetWithoutUserMessages(async () =>
        {
            var sentEvent = await SendEventIfListened(_mediaPlayer.CurrentTrack,
                () =>
                {
                    var position = _mediaPlayer.PlaybackState.CurrentPosition;
                    return _playObserver.GetMeasurementForNewPosition(position);
                });

            if (sentEvent)
                DisposeTimer();
        });
    }

    private async Task<bool> SendEventIfListened(ITrackModel track, Func<PlayMeasurements> measurementsFactory)
    {
        var measurements = measurementsFactory.Invoke();
        if (measurements == null)
        {
            _analytics.LogEvent("Measurements are null");
            return false;
        }

        if (measurements.Percentage > MinListeningPercentage)
        {
            await _playStatistics.PostListeningEvent(track, measurements);
            _analytics.LogEvent("listening event",
                new Dictionary<string, object>
                {
                    {"trackId", track.Id},
                    {"duration", track.Duration},
                    {"currentSecondsListened", measurements.UniqueSecondsListened},
                    {"playbackStarted", measurements.TimestampStart},
                    {"playbackEnded", measurements.TimestampEnd}
                });
            return true;
        }

        return false;
    }
}