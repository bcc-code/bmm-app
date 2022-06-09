using System;
using System.Threading;
using System.Threading.Tasks;
using BMM.Core.Extensions;
using BMM.Core.Implementations.Security.Oidc.Interfaces;

namespace BMM.Core.Implementations.Security.Oidc
{
    public class OidcCredentialsStorage : IOidcCredentialsStorage
    {
        private SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1, 1);
        private readonly ISecureStorageProxy _secureStorage;
        private const string AccessTokenKey = "access_token";
        private const string RefreshTokenKey = "refresh_token";
        private const string AccessTokenExpirationDateKey = "access_token_expiration_date";

        public OidcCredentialsStorage(ISecureStorageProxy secureStorage)
        {
            _secureStorage = secureStorage;
        }

        public Task<string> GetAccessToken()
        {
            return _semaphoreSlim.Run(() => _secureStorage.GetAsync(AccessTokenKey));
        }

        public Task SetAccessToken(string accessToken)
        {
            return _semaphoreSlim.Run(() => _secureStorage.SetAsync(AccessTokenKey, accessToken));
        }

        public Task<string> GetRefreshToken()
        {
            return _semaphoreSlim.Run(() => _secureStorage.GetAsync(RefreshTokenKey));
        }

        public Task SetRefreshToken(string refreshToken)
        {
            return _semaphoreSlim.Run(() => _secureStorage.SetAsync(RefreshTokenKey, refreshToken));
        }

        public Task<DateTime?> GetAccessTokenExpirationDate()
        {
            return _semaphoreSlim.Run(() => _secureStorage.GetDateAsync(AccessTokenExpirationDateKey));
        }

        public Task SetAccessTokenExpirationDate(DateTime time)
        {
            return _semaphoreSlim.Run(() => _secureStorage.SetDateAsync(AccessTokenExpirationDateKey, time));
        }

        public Task FlushStorage()
        {
            return _semaphoreSlim.Run(() => _secureStorage.RemoveAll());
        }
    }
}