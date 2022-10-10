using MvvmCross.Plugin.Messenger;

namespace BMM.Core.Implementations.Downloading.FileDownloader
{   
    public abstract class BaseFileDownloadMessage : MvxMessage
    {
        protected BaseFileDownloadMessage(object sender, int trackId) : base(sender)
        {
            TrackId = trackId;
        }
        
        public int TrackId { get; }
    }
}