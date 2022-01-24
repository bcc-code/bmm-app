using System;
using System.Threading.Tasks;

namespace BMM.Core.Implementations.Security.Oidc
{
    public interface IOidcCredentialsStorage
    {
        Task<string> GetAccessToken();

        Task SetAccessToken(string accessToken);

        Task<string> GetRefreshToken();

        Task SetRefreshToken(string refreshToken);

        Task<DateTime?> GetAccessTokenExpirationDate();

        Task SetAccessTokenExpirationDate(DateTime time);

        Task FlushStorage();
    }
}