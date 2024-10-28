using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Akavache;
using BMM.Core.Helpers;
using BMM.Core.Implementations.FirebaseRemoteConfig;
using BMM.Core.Implementations.Storage;

namespace BMM.Core.Implementations.Connection
{
    public class AkavacheBlobSettingsStorage : ISettingsStorage
    {
        private readonly IFirebaseRemoteConfig _config;

        public AkavacheBlobSettingsStorage(IFirebaseRemoteConfig config)
        {
            _config = config;
        }

        public async Task<bool> GetPlayInChronologicalOrderEnabled() => AppSettings.PlayInChronologicalOrderEnabled ?? _config.PlayInChronologicalOrderEnabledDefaultSetting;
        public async Task<bool> GetAutoplayEnabled() => AppSettings.AutoplayEnabled ?? _config.AutoplayEnabledDefaultSetting;

        public async Task<bool> GetStreakHidden() => AppSettings.StreakHidden;
        public async Task<bool> GetBibleStudyBadgeEnabled() => _config.IsBadgesFeatureEnabled && AppSettings.BibleStudyBadgeEnabled;

        public async Task<bool> GetBibleStudyOnHomeEnabled() => AppSettings.BibleStudyOnHomeEnabled;

        public async Task<bool> GetMobileNetworkDownloadAllowed() => AppSettings.MobileDownloadEnabled;
        public async Task<bool> GetPushNotificationsAllowed() => AppSettings.PushNotificationsEnabled;
        public async Task<bool> UseExternalStorage() => AppSettings.UseExternalStorage;

        public async Task SetStorageLocation(bool isExternalStorage) => AppSettings.UseExternalStorage = isExternalStorage;
        public async Task SetMobileNetworkDownloadAllowed(bool mobileNetworkAllowed) => AppSettings.MobileDownloadEnabled = mobileNetworkAllowed;
        public async Task SetPushNotificationsAllowed(bool pushNotificationsAllowed) => AppSettings.PushNotificationsEnabled = pushNotificationsAllowed;
        public async Task SetAutoplayEnabled(bool enabled) => AppSettings.AutoplayEnabled = enabled;
        public async Task SetPlayInChronologicalOrderEnabled(bool enabled) => AppSettings.PlayInChronologicalOrderEnabled = enabled;
        public async Task SetStreakHidden(bool streakHidden) => AppSettings.StreakHidden = streakHidden;
        public async Task SetBibleStudyBadgeEnabled(bool isEnabled) => AppSettings.BibleStudyBadgeEnabled = isEnabled;
        public async Task SetBibleStudyOnHomeEnabled(bool isEnabled) => AppSettings.BibleStudyOnHomeEnabled = isEnabled;
    }
}