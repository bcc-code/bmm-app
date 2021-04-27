using System.Linq;

namespace BMM.Core.Implementations.Languages
{
    public class EnvironmentLanguageReader
    {
        public string ReadLanguageFromEnvironment(string[] availableLanguages)
        {
            var currentCulture = System.Globalization.CultureInfo.CurrentCulture;
            var currentLanguage = currentCulture.TwoLetterISOLanguageName;

            return availableLanguages.Contains(currentLanguage)
                ? currentLanguage
                : ContentLanguageManager.EnglishIsoCode;
        }
    }
}