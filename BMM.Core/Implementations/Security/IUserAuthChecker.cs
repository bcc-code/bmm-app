using System.Threading.Tasks;

namespace BMM.Core.Implementations.Security
{
    public interface IUserAuthChecker
    {
        Task<bool> IsUserAuthenticated();
    }
}
