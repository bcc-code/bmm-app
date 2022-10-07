using MvvmCross.Plugin.Messenger;

namespace BMM.Core.Implementations.Downloading.FileDownloader
{
    public class FileDownloadStartedMessage : BaseFileDownloadMessage
    {
        public FileDownloadStartedMessage(object sender, int trackId) : base(sender, trackId)
        { }
    }
}