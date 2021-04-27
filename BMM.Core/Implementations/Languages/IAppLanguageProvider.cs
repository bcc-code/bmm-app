using System.Globalization;

namespace BMM.Core.Implementations.Languages
{
    public interface IAppLanguageProvider
    {
        string GetAppLanguage();

        void InitializeAtStartup(string language);

        void ChangeAppLanguage(CultureInfo culture);
    }
}
