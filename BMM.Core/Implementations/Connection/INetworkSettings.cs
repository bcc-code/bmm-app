using System.Threading.Tasks;

namespace BMM.Core.Implementations.Connection
{
    public interface INetworkSettings
    {
        Task<bool> GetMobileNetworkDownloadAllowed();

        Task<bool> GetPushNotificationsAllowed();
    }
}