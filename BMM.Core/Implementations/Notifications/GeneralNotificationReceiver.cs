using System;
using BMM.Core.Implementations.Notifications.Data;
using BMM.Core.Implementations.UI;

namespace BMM.Core.Implementations.Notifications
{
    public class GeneralNotificationReceiver : IReceive<GeneralNotification>
    {
        private readonly IUriOpener _uriOpener;

        public GeneralNotificationReceiver(IUriOpener uriOpener)
        {
            _uriOpener = uriOpener;
        }

        public void UserClickedRemoteNotification(GeneralNotification notification)
        {
            if (!string.IsNullOrEmpty(notification.ActionUrl))
                _uriOpener.OpenUri(new Uri(notification.ActionUrl));
        }

        public void OnNotificationReceived(GeneralNotification notification)
        {
        }
    }
}