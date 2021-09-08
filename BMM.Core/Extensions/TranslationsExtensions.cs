using BMM.Core.Translation;
using MvvmCross.Localization;

namespace BMM.Core.Extensions
{
    public static class TranslationsExtensions
    {
        public static string ConvertPlaylistAuthorToLabel(this IMvxLanguageBinder mvxLanguageBinder, string authorName)
        {
            if (string.IsNullOrEmpty(authorName))
                return string.Empty;

            return string.Format(mvxLanguageBinder.GetText(Translations.MyContentViewModel_ByFormat), authorName);
        }
    }
}