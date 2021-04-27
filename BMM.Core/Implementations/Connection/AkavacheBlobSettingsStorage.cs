using System.Reactive.Linq;
using System.Threading.Tasks;
using Akavache;
using BMM.Core.Helpers;
using BMM.Core.Implementations.FirebaseRemoteConfig;

namespace BMM.Core.Implementations.Connection
{
    public class AkavacheBlobSettingsStorage : ISettingsStorage
    {
        private readonly IBlobCache _storage;
        private readonly IFirebaseRemoteConfig _config;

        public AkavacheBlobSettingsStorage(IBlobCache storage, IFirebaseRemoteConfig config)
        {
            _storage = storage;
            _config = config;
        }

        public async Task<bool> GetAutoplayEnabled()
        {
            var result = await _storage.GetOrCreateObject<bool?>(StorageKeys.AutoplayEnabled, () => null);
            return result ?? _config.AutoplayEnabledDefaultSetting;
        }

        public async Task<bool> GetMobileNetworkDownloadAllowed()
        {
            return await _storage.GetOrCreateObject(StorageKeys.MobileDownloadEnabled, () => GlobalConstants.MobileDownloadEnabledDefault);
        }

        public async Task<bool> GetPushNotificationsAllowed()
        {
            return await _storage.GetOrCreateObject(StorageKeys.PushNotificationsEnabled, () => GlobalConstants.PushNotificationsEnabledDefault);
        }

        public async Task<bool> UseExternalStorage()
        {
            return await _storage.GetOrCreateObject(StorageKeys.UseExternalStorage, () => GlobalConstants.UseExternalStorageDefault);
        }

        public async Task SetStorageLocation(bool isExternalStorage)
        {
            await _storage.InsertObject(StorageKeys.UseExternalStorage, isExternalStorage);
        }

        public async Task SetMobileNetworkDownloadAllowed(bool mobileNetworkAllowed)
        {
            await _storage.InsertObject(StorageKeys.MobileDownloadEnabled, mobileNetworkAllowed);
        }

        public async Task SetPushNotificationsAllowed(bool pushNotificationsAllowed)
        {
            await _storage.InsertObject(StorageKeys.PushNotificationsEnabled, pushNotificationsAllowed);
        }

        public async Task SetAutoplayEnabled(bool autoplayEnabled)
        {
            await _storage.InsertObject(StorageKeys.AutoplayEnabled, autoplayEnabled);
        }
    }
}