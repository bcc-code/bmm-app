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
    public class StreakMessageValueConverter : MvxValueConverter<ListeningStreak, string>
    {
        private IBMMLanguageBinder BMMLanguageBinder => BMMLanguageBinderLocator.TextSource;

        protected override string Convert(ListeningStreak week, Type targetType, object parameter, CultureInfo culture)
        {
            if (week.IsPerfectWeek)
                return BMMLanguageBinder[Translations.Streak_MessagePerfect];

            return BMMLanguageBinder[Translations.Streak_Message];
        }
    }
}