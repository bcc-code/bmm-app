using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Media;
using Android.Net;
using AndroidX.Core.App;
using BMM.Core.Implementations.Analytics;
using BMM.UI.Droid.Application.Activities;
using BMM.Core.Implementations.Notifications;
using BMM.Core.Implementations.Notifications.Data;
using BMM.UI.Droid.Utils;
using MvvmCross.Base;

namespace BMM.UI.Droid.Application.Implementations.Notifications
{
    public class NotificationDisplayer : INotificationDisplayer
    {
        private readonly IMvxMainThreadAsyncDispatcher _mainThreadDispatcher;
        private readonly NotificationChannelBuilder _channelBuilder;
        private readonly IAnalytics _analytics;

        public NotificationDisplayer(IMvxMainThreadAsyncDispatcher mainThreadDispatcher, NotificationChannelBuilder channelBuilder, IAnalytics analytics)
        {
            _mainThreadDispatcher = mainThreadDispatcher;
            _channelBuilder = channelBuilder;
            _analytics = analytics;
        }

        public void DisplayNotificationOrPopup(LocalNotification notification)
        {
            _analytics.LogEvent("show local notification", new Dictionary<string, object> {{"title", notification.Title}});
            _mainThreadDispatcher.ExecuteOnMainThreadAsync(() =>
            {
                var context = Android.App.Application.Context;
                var intent = new Intent(context, typeof(MainActivity));
                intent.AddFlags(ActivityFlags.ClearTop);

                var pendingIntent = PendingIntent.GetActivity(context, 0, intent, PendingIntentsUtils.GetImmutable());
                var notificationBuilder = new NotificationCompat.Builder(context, notification.ChannelId);
                notificationBuilder
                    .SetStyle(new NotificationCompat.BigTextStyle().BigText(notification.Message))
                    .SetSound(RingtoneManager.GetDefaultUri(RingtoneType.Notification))
                    .SetContentTitle(notification.Title)
                    .SetSmallIcon(Resource.Drawable.xam_mediaManager_notify_ic)
                    .SetPriority(2) //Max priority
                    .SetContentText(notification.Message)
                    .SetAutoCancel(true)
                    .SetContentIntent(pendingIntent);

                var notificationManager = (NotificationManager)context.GetSystemService(Context.NotificationService);
                _channelBuilder.EnsureChannel(notificationManager,
                    notification.ChannelId,
                    () => new NotificationChannel(notification.ChannelId, notification.ChannelName, NotificationImportance.Low)
                    {
                        Description = notification.ChannelDescription
                    });
                notificationManager.Notify(0, notificationBuilder.Build());
            });
        }
    }
}