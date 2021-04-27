using MvvmCross.Plugin.Messenger;

namespace BMM.Core.Implementations.Downloading.DownloadQueue
{
    public class DownloadQueueChangedMessage : MvxMessage
    {
        public DownloadQueueChangedMessage(object sender) : base(sender)
        { }
    }
}