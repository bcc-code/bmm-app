using MvvmCross.Plugin.Messenger;

namespace BMM.Core.Messages
{
    public class PushNotificationsStatusChangedMessage : MvxMessage
    {
        public PushNotificationsStatusChangedMessage(object sender, bool enabled)
            : base(sender)
        {
            Enabled = enabled;
        }

        public readonly bool Enabled;
    }
}
