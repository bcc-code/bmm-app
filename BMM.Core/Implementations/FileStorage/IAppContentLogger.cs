using System.Threading.Tasks;

namespace BMM.Core.Implementations.FileStorage
{
    public interface IAppContentLogger
    {
        Task LogAppContent(string eventName);
    }
}