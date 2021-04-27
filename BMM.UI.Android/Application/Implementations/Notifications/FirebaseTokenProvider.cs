using System.Threading.Tasks;
using BMM.Core.Implementations.Notifications;
using Firebase.Iid;

namespace BMM.UI.Droid.Application.Implementations.Notifications
{
    public class FirebaseTokenProvider : INotificationSubscriptionTokenProvider
    {
        // todo should replaced when the bindings are ready: https://firebase.google.com/docs/reference/android/com/google/firebase/messaging/FirebaseMessaging#getToken()
        public Task<string> GetToken()
        {
            return Task.FromResult(FirebaseInstanceId.Instance.Token);
        }
    }
}
