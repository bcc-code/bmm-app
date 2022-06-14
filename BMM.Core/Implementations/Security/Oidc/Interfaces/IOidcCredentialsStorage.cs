using System.Threading.Tasks;

namespace BMM.Core.Implementations.Security.Oidc.Interfaces
{
    public interface IOidcCredentialsStorage
    {
        Task<string> GetAccessToken();

        Task SetAccessToken(string accessToken);

        Task<string> GetRefreshToken();

        Task SetRefreshToken(string refreshToken);

        Task FlushStorage();
    }
}