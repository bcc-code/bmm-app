using BMM.Api.Abstraction;
using BMM.Core.Implementations.Languages;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BMM.Core.Implementations.Analytics
{
    class LanguagesLogger : ILanguagesLogger
    {
        private readonly IAnalytics _analytics;
        private readonly IAppLanguageProvider _appLanguageProvider;
        private readonly IContentLanguageManager _contentLanguageManager;

        public LanguagesLogger(
            IAnalytics analytics,
            IAppLanguageProvider appLanguageProvider,
            IContentLanguageManager contentLanguageManager
            )
        {
            _analytics = analytics;
            _appLanguageProvider = appLanguageProvider;
            _contentLanguageManager = contentLanguageManager;
        }

        public async Task LogAppAndContentLanguages(string eventName)
        {
            var appLanguage = _appLanguageProvider.GetAppLanguage();
            var contentLanguages = await _contentLanguageManager.GetContentLanguages();

            _analytics.LogEvent(eventName,
                new Dictionary<string, object>
                {
                    { "AppLanguge", appLanguage },
                    { "ContentLanguages", string.Join(",", contentLanguages.ToArray()) }
                });
        }
    }
}
