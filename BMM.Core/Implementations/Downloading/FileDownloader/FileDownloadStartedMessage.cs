using MvvmCross.Plugin.Messenger;

namespace BMM.Core.Implementations.Downloading.FileDownloader
{
    public class FileDownloadStartedMessage : MvxMessage
    {
        public FileDownloadStartedMessage(object sender)
            : base(sender)
        { }
    }
}