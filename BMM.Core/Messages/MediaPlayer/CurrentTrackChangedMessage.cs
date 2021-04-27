using BMM.Api.Abstraction;
using MvvmCross.Plugin.Messenger;

namespace BMM.Core.Messages.MediaPlayer
{
    public class CurrentTrackChangedMessage : MvxMessage
    {
        public CurrentTrackChangedMessage(object sender) : base(sender)
        {
        }

        public ITrackModel CurrentTrack;
    }
}
