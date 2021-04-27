using BMM.Core.NewMediaPlayer;
using MvvmCross.Plugin.Messenger;

namespace BMM.Core.Messages.MediaPlayer
{
    public class TrackCompletedMessage : MvxMessage
    {
        public TrackCompletedMessage(object sender) : base(sender)
        { }

        public int NumberOfTracksInQueue { get; set; }
        public PlayStatus PlayStatus { get; set; }
    }
}
