using MvvmCross.Plugin.Messenger;

namespace BMM.Core.Implementations.Downloading.FileDownloader
{
    public class QueueFinishedMessage : MvxMessage
    {
        public bool Succeeded { get; }

        public QueueFinishedMessage(object sender, bool succeeded) : base(sender)
        {
            Succeeded = succeeded;
        }
    }
}