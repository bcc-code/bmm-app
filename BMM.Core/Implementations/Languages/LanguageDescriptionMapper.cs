using System;
using System.Globalization;

namespace BMM.Core.Implementations.Languages
{
    public class LanguageDescriptionMapper
    {
        public LanguageDescription GetFromCode(string languageCode)
        {
            if (languageCode == "yue")
                return new LanguageDescription {EnglishName = "Cantonese", NativeName = "廣東話"};

            try
            {
                var cultureInfo = new CultureInfo(languageCode);
                return new LanguageDescription {EnglishName = cultureInfo.EnglishName, NativeName = cultureInfo.NativeName};
            }
            catch (Exception)
            {
                // If we add another language the app at least shouldn't crash
                return new LanguageDescription {EnglishName = "Unknown language", NativeName = "???"};
            }
        }
    }
}