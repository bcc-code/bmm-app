using System.Threading.Tasks;

namespace BMM.Core.Implementations.Notifications
{
    public interface INotificationSubscriptionTokenProvider
    {
        Task<string> GetToken();
    }
}
