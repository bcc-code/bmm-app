using MvvmCross.Plugin.Messenger;

namespace BMM.Core.Implementations.TrackListenedObservation
{
    public class TrackMarkedAsListenedMessage: MvxMessage
    {
        public int TrackId { get; set; }
        public TrackMarkedAsListenedMessage(object sender) : base(sender)
        { }
    }
}