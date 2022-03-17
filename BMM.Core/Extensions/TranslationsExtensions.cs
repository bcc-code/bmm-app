using System;
using BMM.Api.Implementation.Models;
using BMM.Api.Implementation.Models.Interfaces;
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

        public static string GetTranslationKey(this ITranslationDetailsHolder translationDetailsHolder)
        {
            return $"{translationDetailsHolder.TranslationParent}_{translationDetailsHolder.TranslationId}";
        }

        public static string GetTranslationKey(this Type basicType, string key)
        {
            return $"{basicType.Name}_{key}";
        }
    }
}