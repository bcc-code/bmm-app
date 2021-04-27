using System.Threading.Tasks;
using BMM.Api.Implementation.Models;

namespace BMM.Core.Implementations.Security.Oidc
{
    public interface IOidcAuthService
    {
        /// <summary>
        /// The OpenID connect authentication requires the user to log in in a browser. Afterwards a redirect into the app
        /// is happening with an 'code' parameter in the query of the url.
        /// </summary>
        Task<User> PerformLogin();

        /// <summary>
        /// Removes the credentials and user from storage
        /// </summary>
        Task PerformLogout();

        /// <summary>
        /// Checks whether the user has already an access_token
        /// </summary>
        /// <returns></returns>
        Task<bool> IsAuthenticated();

        /// <summary>
        /// Refreshes the access_token with the refresh_token
        /// </summary>
        Task RefreshAccessTokenWithRetry();
    }
}