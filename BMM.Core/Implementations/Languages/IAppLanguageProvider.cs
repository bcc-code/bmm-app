using System.Globalization;
using BMM.Core.Constants;

namespace BMM.Core.Implementations.Languages
{
    public interface IAppLanguageProvider
    {
        string GetAppLanguage();

        void InitializeAtStartup(string language);

        void ChangeAppLanguage(CultureInfoLanguage cultureInfoLanguage);
    }
}
