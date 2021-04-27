using System.Threading.Tasks;

namespace BMM.Core.Implementations.Connection
{
    public class NetworkSettings : INetworkSettings
    {
        private readonly ISettingsStorage _settingsStorage;

        public NetworkSettings(ISettingsStorage settingsStorage)
        {
            _settingsStorage = settingsStorage;
        }

        public virtual async Task<bool> GetMobileNetworkDownloadAllowed()
        {
            return await _settingsStorage.GetMobileNetworkDownloadAllowed();
        }

        public virtual async Task<bool> GetPushNotificationsAllowed()
        {
            return await _settingsStorage.GetPushNotificationsAllowed();
        }
    }
}