using System;
using System.Threading.Tasks;

namespace BMM.Core.Implementations.Security.Oidc
{
    public class OidcCredentialsStorage : IOidcCredentialsStorage
    {
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
            return _secureStorage.GetAsync(AccessTokenKey);
        }

        public Task SetAccessToken(string accessToken)
        {
            return _secureStorage.SetAsync(AccessTokenKey, accessToken);
        }

        public Task<string> GetRefreshToken()
        {
            return _secureStorage.GetAsync(RefreshTokenKey);
        }

        public Task SetRefreshToken(string refreshToken)
        {
            return _secureStorage.SetAsync(RefreshTokenKey, refreshToken);
        }

        public Task<DateTime?> GetAccessTokenExpirationDate()
        {
            return _secureStorage.GetDateAsync(AccessTokenExpirationDateKey);
        }

        public Task SetAccessTokenExpirationDate(DateTime time)
        {
            return _secureStorage.SetDateAsync(AccessTokenExpirationDateKey, time);
        }

        public void FlushStorage()
        {
            _secureStorage.RemoveAll();
        }
    }
}