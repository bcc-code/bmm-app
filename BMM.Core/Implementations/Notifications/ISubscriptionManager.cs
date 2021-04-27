using System.Threading.Tasks;

namespace BMM.Core.Implementations.Notifications
{
    public interface ISubscriptionManager
    {
        Task UpdateSubscriptionAndRetry();
    }
}

