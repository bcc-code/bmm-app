using BMM.Core.Implementations.Exceptions;
using BMM.Core.Implementations.Notifications;
using Firebase.CloudMessaging;
using MvvmCross;

namespace BMM.UI.iOS.Implementations.Notifications
{
    public class FirebaseMessagingDelegate : MessagingDelegate
    {
        public override void DidReceiveRegistrationToken(Messaging messaging, string fcmToken)
        {
            var handler = Mvx.IoCProvider.Resolve<IExceptionHandler>();
            var manager = Mvx.IoCProvider.Resolve<ISubscriptionManager>();
            handler.FireAndForgetWithoutUserMessages(manager.UpdateSubscriptionAndRetry);
        }
    }
}