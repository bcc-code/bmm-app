using MvvmCross.Plugin.Messenger;

namespace BMM.Core.Implementations.Downloading.FileDownloader
{
    public class FileDownloadCompletedMessage : MvxMessage
    {
        public FileDownloadCompletedMessage(object sender) : base(sender)
        {
        }
    }
}