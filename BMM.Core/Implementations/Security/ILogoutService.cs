using System.Threading.Tasks;

namespace BMM.Core.Implementations.Security
{
    public interface ILogoutService
    {
        Task PerformLogout();
    }
}