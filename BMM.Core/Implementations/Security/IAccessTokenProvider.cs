using System.Threading.Tasks;

namespace BMM.Core.Implementations.Security
{
    public interface IAccessTokenProvider
    {
        Task<string> GetAccessToken();
        Task<bool> IsAccessTokenValid();
    }
}