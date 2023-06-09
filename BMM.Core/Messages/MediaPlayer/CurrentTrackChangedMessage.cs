using BMM.Api.Abstraction;
using MvvmCross.Plugin.Messenger;

namespace BMM.Core.Messages.MediaPlayer
{
    public class CurrentTrackChangedMessage : MvxMessage
    {
        public CurrentTrackChangedMessage(
            ITrackModel currentTrack,
            long startingPositionMs,
            object sender) : base(sender)
        {
            CurrentTrack = currentTrack;
            StartingPositionMs = startingPositionMs;
        }

        public ITrackModel CurrentTrack { get; }
        public long StartingPositionMs { get; }
    }
}
