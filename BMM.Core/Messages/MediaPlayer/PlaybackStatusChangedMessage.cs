using BMM.Core.NewMediaPlayer.Abstractions;
using MvvmCross.Plugin.Messenger;

namespace BMM.Core.Messages.MediaPlayer
{
    public class PlaybackStatusChangedMessage : MvxMessage
    {
        public PlaybackStatusChangedMessage(object sender, IPlaybackState playbackState) : base(sender)
        {
            PlaybackState = playbackState;
        }

        public IPlaybackState PlaybackState { get; }
    }

    public class PlaybackPositionChangedMessage : MvxMessage
    {
        public PlaybackPositionChangedMessage(object sender) : base(sender)
        { }

        public long CurrentPosition { get; set; }
        public long BufferedPosition { get; set; }
    }

    public class PlaybackSeekedMessage : MvxMessage
    {
        public PlaybackSeekedMessage(object sender) : base(sender)
        { }

        public long CurrentPosition { get; set; }
        public long SeekedPosition { get; set; }
    }

    public class RepeatModeChangedMessage : MvxMessage
    {
        public RepeatModeChangedMessage(object sender) : base(sender)
        {
        }

        public RepeatType RepeatType { get; set; }
    }

    public class ShuffleModeChangedMessage : MvxMessage
    {
        public ShuffleModeChangedMessage(object sender) : base(sender)
        {
        }

        public bool IsShuffleEnabled { get; set; }
    }

    public class SkippedTrackMessage : MvxMessage
    {
        public SkippedTrackMessage(object sender) : base(sender)
        {
        }
        public int TrackId { get; set; }
    }
}
