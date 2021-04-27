using Android.App;
using BMM.Core.Implementations.Analytics;
using BMM.Core.Implementations.Exceptions;
using BMM.Core.Implementations.Notifications;
using BMM.UI.Droid.Application.Helpers;
using BMM.UI.Droid.Application.Implementations.Notifications;
using Firebase.Messaging;
using MvvmCross;

namespace Bmm.UI.Droid.Application.Implementations.Notifications
{
    [Service(Name = "bmm.BmmMessagingService", Exported = false), IntentFilter(new[] {"com.google.firebase.MESSAGING_EVENT"})]
    public class BmmMessagingService : FirebaseMessagingService
    {
        /// <summary>
        /// Notification messages are only received by this method if the app is in foreground.
        /// Read about data messages vs notification message: https://firebase.google.com/docs/cloud-messaging/concept-options
        /// </summary>
        public override void OnMessageReceived(RemoteMessage message)
        {
            SetupHelper.EnsureInitialized();

            if (message.GetNotification() != null)
            {
                var analytics = Mvx.IoCProvider.Resolve<IAnalytics>();
                analytics.LogEvent("Received and ignored notification while using the app");
            }
            else
            {
                // it's a data message that's intended to be processed by the app
                var handler = Mvx.IoCProvider.Resolve<INotificationHandler>();
                handler.OnNotificationReceived(new RemoteMessageNotification(message));
            }
        }

        /// <summary>
        /// This method is just called:
        /// 1) When a new token is generated on initial app startup
        /// 2) Whenever an existing token is changed
        ///     A) App is restored to a new device
        ///     B) User uninstalls/reinstalls the app
        ///     C) User  clears app data)
        /// </summary>
        public override void OnNewToken(string token)
        {
            SetupHelper.EnsureInitialized();
            var handler = Mvx.IoCProvider.Resolve<IExceptionHandler>();
            var manager = Mvx.IoCProvider.Resolve<ISubscriptionManager>();
            handler.FireAndForgetWithoutUserMessages(manager.UpdateSubscriptionAndRetry);
        }
    }
}