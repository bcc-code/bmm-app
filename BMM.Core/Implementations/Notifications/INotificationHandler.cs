namespace BMM.Core.Implementations.Notifications
{
    public interface INotificationHandler
    {
        void OnNotificationReceived(IPlatformNotification message);

        /// <summary>
        /// This message should be called only if the user clicked on a notification and we should do a special action that is related to the notification.
        /// </summary>
        void UserClickedNotification(IPlatformNotification message);

        void OnNotificationReceivedInForeground(IPlatformNotification notification);

        bool WillNotificationStartPlayer(IPlatformNotification notification);
    }
}