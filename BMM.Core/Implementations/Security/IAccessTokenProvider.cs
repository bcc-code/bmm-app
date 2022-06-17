using System;
using System.Threading.Tasks;
using BMM.Core.Models.App;

namespace BMM.Core.Implementations.Security
{
    public interface IAccessTokenProvider
    {
        /// <summary>
        /// Is is not guaranteed that this access token is valid.
        /// Only use in places, where async operation cannot be properly awaited. 
        /// </summary>
        string AccessToken { get; }
        AccessTokenState CheckAccessTokenState();
        Task<string> GetAccessToken();
        Task Initialize();
        Task UpdateAccessTokenIfNeeded();
        DateTime GetTokenExpirationDate();
    }
}