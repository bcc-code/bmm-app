using System.Collections.Generic;
using System.Threading.Tasks;

namespace BMM.Api.Abstraction
{
    public interface IContentLanguageManager
    {
        Task<IEnumerable<string>> GetContentLanguages();

        Task SetContentLanguages(IEnumerable<string> languages);

        Task<IEnumerable<string>> GetContentLanguagesIncludingHidden();
    }
}