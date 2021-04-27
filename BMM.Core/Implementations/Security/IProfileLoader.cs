using System.Threading.Tasks;

namespace BMM.Core.Implementations.Security
{
    public interface IProfileLoader
    {
        Task<ProfileLoader.ProfileResponse> LoadProfile();
    }
}