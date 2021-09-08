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
    public class PerfectWeekCountValueConverter : MvxValueConverter<ListeningStreak, string>
    {
        private IBMMLanguageBinder BMMLanguageBinder => BMMLanguageBinderLocator.TextSource;

        protected override string Convert(ListeningStreak streak, Type targetType, object parameter, CultureInfo culture)
        {
            if (streak.NumberOfPerfectWeeks <= 0)
                return BMMLanguageBinder[Translations.Streak_PerfectWeekCountNone];

            string translationKey = streak.NumberOfPerfectWeeks > 1
                ? Translations.Streak_PerfectWeekCountPlural
                : Translations.Streak_PerfectWeekCountSingular;

            return BMMLanguageBinder.GetText(translationKey, streak.NumberOfPerfectWeeks);
        }
    }
}