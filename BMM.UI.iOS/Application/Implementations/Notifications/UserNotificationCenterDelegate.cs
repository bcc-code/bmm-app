using System;
using BMM.Api.Framework;
using BMM.Core.Implementations.Notifications;
using UserNotifications;

namespace BMM.UI.iOS.Implementations.Notifications
{
    /// <summary>
    /// Unlike Android, silent push notifications (also called "data message") do not wake up the app if the app is <see cref="UIApplicationState.Inactive"/>.
    /// (Actually there are some scenarios when it is still activated but it's not reliable.)
    /// </summary>
    public class UserNotificationCenterDelegate : UNUserNotificationCenterDelegate
    {
        private readonly ILogger _logger;
        private readonly INotificationHandler _notificationHandler;

        public UserNotificationCenterDelegate(ILogger logger, INotificationHandler notificationHandler)
        {
            _logger = logger;
            _notificationHandler = notificationHandler;
        }

        public override void WillPresentNotification(UNUserNotificationCenter center, UNNotification notification, Action<UNNotificationPresentationOptions> completionHandler)
        {
            _logger.Info(nameof(UserNotificationCenterDelegate), "WillPresentNotification");
            _notificationHandler.OnNotificationReceivedInForeground(new IosNotification(notification));
            completionHandler(UNNotificationPresentationOptions.Alert);
        }

        public override void DidReceiveNotificationResponse(UNUserNotificationCenter center, UNNotificationResponse response, Action completionHandler)
        {
            _logger.Info(nameof(UserNotificationCenterDelegate), "DidReceiveNotificationResponse");
            _notificationHandler.UserClickedNotification(new IosNotification(response.Notification));
            completionHandler();
        }
    }
}