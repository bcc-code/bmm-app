using System.Threading.Tasks;
using BMM.Core.Implementations.Notifications;
using Firebase.InstanceID;

namespace BMM.UI.iOS.Implementations.Notifications
{
    public class FirebaseTokenProvider : INotificationSubscriptionTokenProvider
    {
        public async Task<string> GetToken()
        {
            var instance = await InstanceId.SharedInstance.GetInstanceIdAsync();
            return instance.Token;
        }
    }
}
