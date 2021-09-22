using BMM.Api.Abstraction;
using MvvmCross.Plugin.Messenger;

namespace BMM.Core.Messages.MediaPlayer
{
    public class CurrentTrackChangedMessage : MvxMessage
    {
        public CurrentTrackChangedMessage(
            ITrackModel currentTrack,
            object sender) : base(sender)
        {
            CurrentTrack = currentTrack;
        }

        public ITrackModel CurrentTrack { get; }
    }
}
