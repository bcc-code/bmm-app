using MvvmCross.Plugin.Messenger;

namespace BMM.Core.Messages
{
    public class LoggedOutMessage: MvxMessage
    {
        public LoggedOutMessage(object sender) : base(sender)
        {
        }
    }
}