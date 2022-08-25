using System;
using System.Globalization;
using BMM.Api.Implementation.Models;
using BMM.Core.Helpers;
using BMM.Core.Implementations.Localization;
using BMM.Core.Implementations.Localization.Interfaces;
using BMM.Core.Translation;
using MvvmCross.Converters;
using MvvmCross.Localization;

namespace BMM.Core.ValueConverters
{
    public class DaysInARowMessageValueConverter : MvxValueConverter<ListeningStreak, string>
    {
        private IBMMLanguageBinder BMMLanguageBinder => BMMLanguageBinderLocator.TextSource;

        protected override string Convert(ListeningStreak streak, Type targetType, object parameter, CultureInfo culture)
        {
            string translationKey = streak.DaysInARow == 1
                ? Translations.Streak_DaysInARowSingular
                : Translations.Streak_DaysInARowPlural;

            return BMMLanguageBinder.GetText(translationKey, streak.DaysInARow);
        }
    }
}