using System;
using System.Threading.Tasks;
using Microsoft.Maui.Storage;

namespace BMM.Core.Implementations.Security
{
    public class SecureStorageProxy : ISecureStorageProxy
    {
        public Task SetAsync(string key, string value)
        {
            return SecureStorage.SetAsync(key, value);
        }

        public Task<string> GetAsync(string key)
        {
            return SecureStorage.GetAsync(key);
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