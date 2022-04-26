using MvvmCross.Plugin.Messenger;

namespace BMM.Core.Messages.MediaPlayer
{
    public class PlaybackSpeedChangeRequestedMessage : MvxMessage
    {
        public PlaybackSpeedChangeRequestedMessage(object sender) : base(sender)
        {
        }
    }
}