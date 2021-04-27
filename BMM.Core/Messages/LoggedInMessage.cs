using MvvmCross.Plugin.Messenger;

namespace BMM.Core
{
    public class LoggedInMessage: MvxMessage
    {
        public LoggedInMessage(object sender)
            : base(sender)
        {
        }
    }
}
