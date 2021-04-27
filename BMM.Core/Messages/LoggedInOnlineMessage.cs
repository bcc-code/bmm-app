using MvvmCross.Plugin.Messenger;

namespace BMM.Core.Messages
{
    public class LoggedInOnlineMessage: MvxMessage
    {
        public LoggedInOnlineMessage (object sender)
            : base(sender)
        {
        }
    }
}

