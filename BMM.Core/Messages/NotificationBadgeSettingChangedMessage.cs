using MvvmCross.Plugin.Messenger;

namespace BMM.Core.Messages;

public class NotificationBadgeSettingChangedMessage : MvxMessage
{
    public NotificationBadgeSettingChangedMessage(object sender) : base(sender)
    {
    }
}