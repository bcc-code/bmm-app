using System.Threading.Tasks;

namespace BMM.Core.Implementations.Security
{
    public interface IAccessTokenProvider
    {
        /// <summary>
        /// Is is not guaranteed that this access token is valid.
        /// Only use in places, there async operation cannot be properly awaited. 
        /// </summary>
        string AccessToken { get; }
        bool CheckIsTokenValid(string accessToken);
        Task<string> GetAccessToken();
        Task<bool> IsAccessTokenValid();
    }
}