using System;
using Android.App;
using Android.OS;

namespace BMM.UI.Droid.Application.NewMediaPlayer.Notification
{
    public class NotificationChannelBuilder
    {
        /// <summary>
        /// Creates the notification channel if it needs to be created.
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="notificationChannelId"></param>
        /// <param name="channel">It needs to be a func because creating the <see cref="NotificationChannel"/> creates an exception on older Android versions. See <see cref="ShouldCreateNowPlayingChannel"/></param>
        public void EnsureChannel(NotificationManager manager, string notificationChannelId, Func<NotificationChannel> channel)
        {
            if (ShouldCreateNowPlayingChannel(manager, notificationChannelId))
            {
                CreateNowPlayingChannel(manager, channel);
            }
        }

        private bool ShouldCreateNowPlayingChannel(NotificationManager notificationManager, string notificationChannelId)
        {
            return Build.VERSION.SdkInt >= BuildVersionCodes.O && !NowPlayingChannelExists(notificationManager, notificationChannelId);
        }

        private bool NowPlayingChannelExists(NotificationManager notificationManager, string channelId)
        {
            return notificationManager.GetNotificationChannel(channelId) != null;
        }

        private void CreateNowPlayingChannel(NotificationManager notificationManager, Func<NotificationChannel> channel)
        {
            notificationManager.CreateNotificationChannel(channel.Invoke());
        }
    }
}