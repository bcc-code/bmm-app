using System.Threading.Tasks;

namespace BMM.Core.Implementations.Analytics
{
    public interface ILanguagesLogger
    {
        Task LogAppAndContentLanguages(string eventName);
    }
}
