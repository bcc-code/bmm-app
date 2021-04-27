using System.Diagnostics;
using BMM.Api.Framework;
using BMM.Core.Messages.MediaPlayer;

namespace BMM.Core.Implementations.PlayObserver
{
    public class LivestreamPlayStatisticsDecorator : PlayStatisticsDecorator
    {
        private readonly ILogger _logger;

        private readonly Stopwatch _positionInTrack;
        private const string Tag = nameof(LivestreamPlayStatisticsDecorator);

        public LivestreamPlayStatisticsDecorator(IPlayStatistics playStatistics, ILogger logger) : base(playStatistics)
        {
            _logger = logger;
            _positionInTrack = new Stopwatch();
        }

        public override void OnCurrentTrackChanged(CurrentTrackChangedMessage message)
        {
            base.OnCurrentTrackChanged(message);

            _positionInTrack.Restart();
        }

        public override void OnTrackCompleted(TrackCompletedMessage message)
        {
            if (IsLivePlayback())
            {
                _logger.Error(Tag, "OnTrackCompleted triggered for a live playback");
            }

            base.OnTrackCompleted(message);
        }

        public override void OnPlaybackStateChanged(IPlaybackState state)
        {
            if (IsLivePlayback())
            {
                // Since the position for LivePlayback is not correct, we track the position using a stopwatch
                state = new PlaybackState(state) {CurrentPosition = _positionInTrack.ElapsedMilliseconds};
            }

            base.OnPlaybackStateChanged(state);
        }

        public override void OnSeeked(double currentPosition, double seekedPosition)
        {
            if (IsLivePlayback())
            {
                // Seeking is not supported for Livestream. Therefore we swallow that event and instead rely on the data from OnPlaybackStateChanged
            }
            else
            {
                base.OnSeeked(currentPosition, seekedPosition);
            }
        }

        private bool IsLivePlayback()
        {
            // The Media Player could run into an unexpected problem and then the user can click on the play/pause button,
            // even if no media is playing. That's why we have to check if CurrentTrack is not null.
            return CurrentTrack != null && CurrentTrack.IsLivePlayback;
        }
    }
}