using BMM.Api.Abstraction;
using BMM.Core.Implementations.PlayObserver.Model;
using MvvmCross.Plugin.Messenger;

namespace BMM.Core.Messages
{
    public class StreakTrackCompletedMessage : MvxMessage
    {
        public StreakTrackCompletedMessage(object sender) : base(sender)
        { }

        public ITrackModel Track { get; set; }

        public PlayMeasurements Measurements { get; set; }
    }
}
