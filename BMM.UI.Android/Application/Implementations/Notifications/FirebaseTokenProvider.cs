using System.Threading.Tasks;
using Android.Gms.Extensions;
using BMM.Core.Implementations.Notifications;
using Firebase.Iid;
using Firebase.Messaging;

namespace BMM.UI.Droid.Application.Implementations.Notifications
{
    public class FirebaseTokenProvider : INotificationSubscriptionTokenProvider
    {
        public async Task<string> GetToken()
        {
            var tokenObject = await FirebaseMessaging.Instance.GetToken();
            return tokenObject.ToString();
        }
    }
}