using System;
using BMM.Core.Helpers;
using BMM.Core.Implementations.Localization;
using BMM.Core.Implementations.Localization.Interfaces;
using BMM.Core.Translation;
using BMM.Core.ViewModels;
using MvvmCross.Converters;
using MvvmCross.Localization;

namespace BMM.Core.ValueConverters
{
    public class TimeSpanToCountdownValueConverter: MvxValueConverter<TimeSpan?, string>
    {
        private IBMMLanguageBinder BMMLanguageBinder => BMMLanguageBinderLocator.TextSource;

        protected override string Convert (TimeSpan? timeLeft, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (timeLeft == null)
                return "";

            if (timeLeft.Value.TotalHours > 1)
            {
                return BMMLanguageBinder.GetText(Translations.ExploreNewestViewModel_CountdownHours, timeLeft.Value.Hours);
            }

            return $"{timeLeft.Value.Minutes}:{timeLeft.Value.Seconds.ToString().PadLeft(2, '0')}";
        }
    }
}