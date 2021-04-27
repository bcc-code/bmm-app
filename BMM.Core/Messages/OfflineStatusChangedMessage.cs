using MvvmCross.Plugin.Messenger;

namespace BMM.Core.Messages
{
    public class OfflineStatusChangedMessage: MvxMessage
    {
        public OfflineStatusChangedMessage (object sender, bool offline)
            : base(sender)
        {
            Offline = offline;
        }

        public readonly bool Offline;
    }
}

