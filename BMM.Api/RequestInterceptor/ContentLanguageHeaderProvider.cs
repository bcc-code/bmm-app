using System.Collections.Generic;
using System.Threading.Tasks;
using BMM.Api.Abstraction;
using BMM.Api.Implementation.Constants;

namespace BMM.Api.RequestInterceptor
{
    public class ContentLanguageHeaderProvider : IHeaderProvider
    {
        private readonly IContentLanguageManager _contentLanguageManager;

        public ContentLanguageHeaderProvider(IContentLanguageManager contentLanguageManager)
        {
            _contentLanguageManager = contentLanguageManager;
        }

        public async Task<KeyValuePair<string, string>?> GetHeader()
        {
            var contentLanguages = await _contentLanguageManager.GetContentLanguagesIncludingHidden();
            var languages = string.Join(",", contentLanguages);
            return new KeyValuePair<string, string>(HeadersNames.AcceptLanguage, languages);
        }
    }
}