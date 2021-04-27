using MvvmCross.Plugin.Messenger;

namespace BMM.Core.Messages
{
    public class DownloadedEpisodeRemovedMessage : MvxMessage
    {
        public DownloadedEpisodeRemovedMessage(object sender)
            : base(sender)
        { }
    }
}