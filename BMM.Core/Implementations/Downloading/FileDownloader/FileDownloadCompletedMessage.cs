using MvvmCross.Plugin.Messenger;

namespace BMM.Core.Implementations.Downloading.FileDownloader
{
    public class FileDownloadCompletedMessage : BaseFileDownloadMessage
    {
        public FileDownloadCompletedMessage(object sender, int trackId) : base(sender, trackId)
        {
        }
    }
}