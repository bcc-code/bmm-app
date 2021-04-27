using MvvmCross.Plugin.Messenger;

namespace BMM.Core.Messages
{
    public class AutomaticDownloadChangedMessage : MvxMessage
    {
        public AutomaticDownloadChangedMessage(object sender, string itemSelected)
            : base(sender)
        {
            ItemSelected = itemSelected;
        }

        public readonly string ItemSelected;
    }
}
