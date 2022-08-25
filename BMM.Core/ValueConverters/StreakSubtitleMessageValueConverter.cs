using System;
using System.Globalization;
using BMM.Api.Implementation.Models;
using BMM.Api.Implementation.Models.Enums;
using BMM.Core.Implementations.Localization;
using BMM.Core.Implementations.Localization.Interfaces;
using MvvmCross.Converters;

namespace BMM.Core.ValueConverters
{
    public class StreakSubtitleMessageValueConverter : MvxValueConverter<ListeningStreak, string>
    {
        private IBMMLanguageBinder BMMLanguageBinder => BMMLanguageBinderLocator.TextSource;

        protected override string Convert(ListeningStreak listeningStreak, Type targetType, object parameter, CultureInfo culture)
        {
            if (listeningStreak.HomeScreenText == HomeScreenText.DaysInRow)
            {
                var daysInARowMessageValueConverter = new DaysInARowMessageValueConverter();
                return (string)daysInARowMessageValueConverter.Convert(listeningStreak,
                    typeof(string),
                    null,
                    culture);
            }

            var converter = new PerfectWeekCountValueConverter();
            
            return (string)converter.Convert(listeningStreak,
                typeof(string),
                null,
                culture);
        }
    }
}