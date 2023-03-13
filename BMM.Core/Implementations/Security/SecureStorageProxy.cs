using BMM.Core.Support;

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

        public Task<string> GetAsync(string key)
        {
            return _oldSecureStorage.GetAsync(key);
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