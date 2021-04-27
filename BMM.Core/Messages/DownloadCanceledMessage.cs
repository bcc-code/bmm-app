using MvvmCross.Plugin.Messenger;

namespace BMM.Core.Messages
{
    public class DownloadCanceledMessage : MvxMessage
    {
        public DownloadCanceledMessage(object sender)
            : base(sender)
        {
        }
    }
}
