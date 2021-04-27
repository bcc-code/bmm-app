using System.Threading.Tasks;

namespace BMM.Api.Abstraction
{
    public interface INeedInitialization
    {
        Task InitializeWhenLoggedIn();
    }
}