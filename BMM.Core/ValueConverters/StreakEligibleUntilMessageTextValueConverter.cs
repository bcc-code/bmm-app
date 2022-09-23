using System;
using System.Globalization;
using BMM.Core.Implementations.Localization;
using BMM.Core.Implementations.Localization.Interfaces;
using BMM.Core.Translation;
using MvvmCross.Converters;

namespace BMM.Core.ValueConverters
{
    public class StreakEligibleUntilMessageTextValueConverter : MvxValueConverter<DateTime, string>
    {
        private IBMMLanguageBinder TextSource => BMMLanguageBinderLocator.TextSource;

        protected override string Convert(DateTime eligibleDateTime, Type targetType, object parameter, CultureInfo culture)
        {
            if (eligibleDateTime < DateTime.UtcNow)
                return string.Empty;

            var remainingTime = eligibleDateTime - DateTime.UtcNow;

            return remainingTime.Hours > 0
                ? TextSource.GetText(Translations.Streak_EligibleUntilHoursAndMinutesMessage, remainingTime.Hours, remainingTime.Minutes)
                : TextSource.GetText(Translations.Streak_EligibleUntilMinutesOnlyMessage, remainingTime.Minutes);
        }
    }
}