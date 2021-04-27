using BMM.Core.NewMediaPlayer;

namespace BMM.Core.Messages.MediaPlayer
{
    public interface IPlaybackState
    {
        bool IsPlaying { get; }

        PlayStatus PlayStatus { get; }

        double PlaybackRate { get; }

        bool IsSkipToNextEnabled { get; }

        bool IsSkipToPreviousEnabled { get; }

        long CurrentIndex { get; }

        int QueueLength { get; }

        long CurrentPosition { get; }

        long BufferedPosition { get; }
    }

    public class DefaultPlaybackState : IPlaybackState
    {
        public bool IsPlaying => false;

        public PlayStatus PlayStatus => PlayStatus.Stopped;

        public double PlaybackRate => 0;

        public bool IsSkipToNextEnabled => false;

        public bool IsSkipToPreviousEnabled => false;

        public long CurrentIndex => 0;

        public int QueueLength => 0;

        public long CurrentPosition => 0;

        public long BufferedPosition => 0;
    }

    public class PlaybackState : IPlaybackState
    {
        public PlaybackState()
        {
        }

        /// <summary>
        /// create a clone of another PlaybackState
        /// </summary>
        public PlaybackState(IPlaybackState original)
        {
            IsPlaying = original.IsPlaying;
            PlayStatus = original.PlayStatus;
            PlaybackRate = original.PlaybackRate;
            IsSkipToNextEnabled = original.IsSkipToNextEnabled;
            IsSkipToPreviousEnabled = original.IsSkipToPreviousEnabled;
            CurrentIndex = original.CurrentIndex;
            QueueLength = original.QueueLength;
            CurrentPosition = original.CurrentPosition;
            BufferedPosition = original.BufferedPosition;
        }

        public bool IsPlaying { get; set; }

        public PlayStatus PlayStatus { get; set; }

        public double PlaybackRate { get; set; }

        public bool IsSkipToNextEnabled { get; set; }

        public bool IsSkipToPreviousEnabled { get; set; }

        public long CurrentIndex { get; set; }

        public int QueueLength { get; set; }

        public long CurrentPosition { get; set; }

        public long BufferedPosition { get; set; }
    }
}
