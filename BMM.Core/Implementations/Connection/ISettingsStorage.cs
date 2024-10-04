using System.Threading.Tasks;

namespace BMM.Core.Implementations.Connection
{
    public interface ISettingsStorage
    {
        Task<bool> UseExternalStorage();
        Task SetStorageLocation(bool isExternalStorage);
        Task<bool> GetMobileNetworkDownloadAllowed();
        Task SetMobileNetworkDownloadAllowed(bool mobileNetworkAllowed);
        Task<bool> GetPushNotificationsAllowed();
        Task SetPushNotificationsAllowed(bool pushNotificationsAllowed);

        Task<bool> GetAutoplayEnabled();
        Task<bool> GetPlayInChronologicalOrderEnabled();
        
        Task SetAutoplayEnabled(bool enabled);
        Task SetPlayInChronologicalOrderEnabled(bool enabled);

        Task<bool> GetStreakHidden();

        Task SetStreakHidden(bool streakHidden);
    }
}
