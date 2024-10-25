using MvvmCross.Plugin.Messenger;

namespace BMM.Core.Messages;

public class BadgeCountChangedMessage : MvxMessage
{
    public BadgeCountChangedMessage(object sender) : base(sender)
    {
    }
}