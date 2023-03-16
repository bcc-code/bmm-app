using BMM.Core.Support;
using Microsoft.Maui.Storage;
using Newtonsoft.Json;

namespace BMM.Core.Implementations.Security
{
    public class SecureStorageProxy : ISecureStorageProxy
    {
        private readonly IOldSecureStorage _oldSecureStorage;

        public SecureStorageProxy(IOldSecureStorage oldSecureStorage)
        {
            _oldSecureStorage = oldSecureStorage;
        }
        
        public Task SetAsync(string key, string value)
        {
            return _oldSecureStorage.SetAsync(key, value);
        }

        public async Task SetAsync<TValue>(string settingsKey, TValue value)
        {
            string serialized = value is null
                ? null
                : JsonConvert.SerializeObject(value);

            await SecureStorage.SetAsync(settingsKey, serialized);
        }
        
        public Task<string> GetAsync(string key)
        {
            return _oldSecureStorage.GetAsync(key);
        }

        public async Task<TValue> GetAsync<TValue>(string key, TValue defaultValue = default)
        {
            string value = await SecureStorage.GetAsync(key);
            return value is null
                ? defaultValue
                : JsonConvert.DeserializeObject<TValue>(value);
        }
        
        public void RemoveAll()
        {
            _oldSecureStorage.RemoveAll();
        }

        public async Task<DateTime?> GetDateAsync(string key)
        {
            var timeString = await _oldSecureStorage.GetAsync(key);
            var success = long.TryParse(timeString, out var integerRepresentation);
            if (!success)
                return null;
            return DateTime.FromBinary(integerRepresentation);
        }

        public Task SetDateAsync(string key, DateTime date)
        {
            var timeString = date.ToBinary().ToString();
            return _oldSecureStorage.SetAsync(key, timeString);
        }
    }
}