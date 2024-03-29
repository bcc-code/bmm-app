﻿using BMM.Api.Framework;
using BMM.Core.Implementations.Notifications.Data;
using MvvmCross;

namespace BMM.Core.Implementations.Notifications
{
    /// <summary>
    /// Notifications:
    /// - PodcastNotification (RemoteNotification)
    /// - GeneralNotification (RemoteNotification)
    /// Possible problems:
    /// - Android does not support MessageWindows yet
    /// </summary>
    public class NotificationHandler : INotificationHandler
    {
        private readonly NotificationParser _parser;
        private readonly ILogger _logger;

        public NotificationHandler(NotificationParser parser, ILogger logger)
        {
            _parser = parser;
            _logger = logger;
        }

        private void HandleNotification(INotification notification, NotificationType type)
        {
            switch (notification)
            {
                case PodcastNotification podcastNotification:
                    HandleRemoteNotification(podcastNotification, type);
                    break;
                case GeneralNotification generalNotification:
                    HandleRemoteNotification(generalNotification, type);
                    break;
                case AchievementNotification achievementNotification:
                    HandleRemoteNotification(achievementNotification, type);
                    break;
                default:
                    _logger.Error(nameof(NotificationHandler), "Encountered an unsupported notification");
                    break;
            }
        }

        /// <summary>
        /// This message should be called only if the user clicked on a notification and we're supposed to do a special action that is related to the notification.
        /// </summary>
        public void UserClickedNotification(IPlatformNotification message)
        {
            ParseAndHandleNotification(message, NotificationType.UserClickedNotification);
        }

        public void OnNotificationReceivedInForeground(IPlatformNotification notification)
        {
            ParseAndHandleNotification(notification, NotificationType.ReceivedWhileUsing);
        }

        public bool WillNotificationStartPlayer(IPlatformNotification notification)
        {
            return _parser.ParseNotification(notification) is PodcastNotification;
        }

        public void OnNotificationReceived(IPlatformNotification message)
        {
            ParseAndHandleNotification(message, NotificationType.ReceivedDataMessage);
        }

        private void ParseAndHandleNotification(IPlatformNotification message, NotificationType type)
        {
            var notification =  _parser.ParseNotification(message);
            HandleNotification(notification, type);
        }

        private void HandleRemoteNotification<T>(T notification, NotificationType type) where T : RemoteNotification
        {
            var receiver = Mvx.IoCProvider.Resolve<IReceive<T>>();

            if (type == NotificationType.ReceivedDataMessage)
                receiver.OnNotificationReceived(notification);
            else if (type == NotificationType.ReceivedWhileUsing)
                receiver.OnNotificationReceived(notification);
            else
                receiver.UserClickedRemoteNotification(notification);
        }

        public enum NotificationType
        {
            ReceivedDataMessage,
            ReceivedWhileUsing,
            UserClickedNotification
        }
    }
}