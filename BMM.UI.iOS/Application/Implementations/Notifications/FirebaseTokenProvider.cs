using BMM.Core.Implementations.Notifications;
using Firebase.CloudMessaging;

namespace BMM.UI.iOS.Implementations.Notifications
{
    public class FirebaseTokenProvider : INotificationSubscriptionTokenProvider
    {
        public Task<string> GetToken() => Messaging.SharedInstance.FetchTokenAsync();
    }
}