using System.Collections.Generic;
using BMM.Core.Implementations.Analytics;
using BMM.Core.Implementations.FirebaseRemoteConfig;
using BMM.Core.Implementations.PlayObserver.Model;
using BMM.Core.Messages.MediaPlayer;
using MvvmCross.Plugin.Messenger;

namespace BMM.Core.Implementations.PlayObserver
{
    public class PlayObserverOrchestrator
    {
        private readonly IPlayStatistics _playStatistics;
        private readonly IAnalytics _analytics;
        private readonly IFirebaseRemoteConfig _config;

        private MvxSubscriptionToken _playbackStatusToken;
        private MvxSubscriptionToken _trackChangedToken;
        private MvxSubscriptionToken _trackCompletedToken;
        private MvxSubscriptionToken _playbackSeekedToken;
        private readonly MvxSubscriptionToken _queueChangedToken;
        private readonly MvxSubscriptionToken _currentTrackWillChangeToken;
        private readonly MvxSubscriptionToken _skippedTrackToken;

        public PlayObserverOrchestrator(IMvxMessenger messenger, IPlayStatistics playStatistics, IAnalytics analytics, IFirebaseRemoteConfig config)
        {
            _playStatistics = playStatistics;
            _analytics = analytics;
            _config = config;

            _playbackStatusToken = messenger.Subscribe<PlaybackStatusChangedMessage>(message =>
            {
                Log("PlaybackStatusChanged",
                    new Dictionary<string, object> {{"CurrentPosition", message.PlaybackState.CurrentPosition}, {"PlayStatus", message.PlaybackState.PlayStatus}});
                _playStatistics.OnPlaybackStateChanged(message.PlaybackState);
            });
            _trackChangedToken = messenger.Subscribe<CurrentTrackChangedMessage>(OnCurrentTrackChanged);
            _queueChangedToken = messenger.Subscribe<CurrentQueueChangedMessage>(OnCurrentQueueChanged);
            _trackCompletedToken = messenger.Subscribe<TrackCompletedMessage>(message =>
            {
                Log("TrackCompleted", new Dictionary<string, object>());
                _playStatistics.OnTrackCompleted(message);
            });
            _playbackSeekedToken = messenger.Subscribe<PlaybackSeekedMessage>(message =>
            {
                Log("PlaybackSeeked", new Dictionary<string, object> {{"CurrentPosition", message.CurrentPosition}, {"SeekedPosition", message.SeekedPosition}});
                _playStatistics.OnSeeked(message.CurrentPosition, message.SeekedPosition);
            });
            _currentTrackWillChangeToken = messenger.Subscribe<CurrentTrackWillChangeMessage>(message =>
            {
                _playStatistics.OnCurrentTrackWillChange(message.CurrentPosition, message.PlaybackRate);
            });
            _skippedTrackToken = messenger.Subscribe<SkippedTrackMessage>(message => _playStatistics.OnSkippedTrack(message.TrackId));

            _playStatistics.TriggerClear += () => { _playStatistics.Clear(); };
        }

        private void OnCurrentQueueChanged(CurrentQueueChangedMessage message)
        {
            _playStatistics.OnCurrentQueueChanged(message);
        }

        private void OnCurrentTrackChanged(CurrentTrackChangedMessage message)
        {
            Log("CurrentTrackChanged", new Dictionary<string, object> {{"NewTrack", $"{message.CurrentTrack?.Title}({message.CurrentTrack?.Id})"}});
            if (_playStatistics.CurrentTrack != message.CurrentTrack)
            {
                _playStatistics.OnCurrentTrackChanged(message);
            }
        }

        public PlayMeasurements GetMeasurementForNewPosition(long position)
        {
            return _playStatistics.GetMeasurementForNewPosition(position);
        }

        private void Log(string message, IDictionary<string, object> parameters)
        {
            if (!_config.UseExtendedStreakLogging)
                return;

            parameters.Add("CurrentTrack", $"{_playStatistics.CurrentTrack?.Title}({_playStatistics.CurrentTrack?.Id})");
            parameters.Add("Portions", _playStatistics.PortionsListened.Count);
            parameters.Add("StartOfNextPortion", _playStatistics.StartOfNextPortion);
            _analytics.LogEvent("PlayStatisticsEvents|" + message, parameters);
        }
    }
}