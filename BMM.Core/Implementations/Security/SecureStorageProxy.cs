using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xamarin.Essentials;

namespace BMM.Core.Implementations.Security
{
    public class SecureStorageProxy : ISecureStorageProxy
    {
        public Task SetAsync(string key, string value)
        {
            return SecureStorage.SetAsync(key, value);
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
            return SecureStorage.GetAsync(key);
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
            SecureStorage.RemoveAll();
        }

        public async Task<DateTime?> GetDateAsync(string key)
        {
            var timeString = await SecureStorage.GetAsync(key);
            var success = long.TryParse(timeString, out var integerRepresentation);
            if (!success)
                return null;
            return DateTime.FromBinary(integerRepresentation);
        }

        public Task SetDateAsync(string key, DateTime date)
        {
            var timeString = date.ToBinary().ToString();
            return SecureStorage.SetAsync(key, timeString);
        }
    }
}