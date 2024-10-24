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
        Task<bool> GetBibleStudyBadgeEnabled();
        Task<bool> GetBibleStudyOnHomeEnabled();

        Task SetStreakHidden(bool streakHidden);
        Task SetBibleStudyBadgeEnabled(bool isEnabled);
        Task SetBibleStudyOnHomeEnabled(bool isEnabled);
    }
}
