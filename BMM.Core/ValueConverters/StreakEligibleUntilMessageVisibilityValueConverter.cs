using System;
using System.Globalization;
using BMM.Core.Implementations.Localization;
using BMM.Core.Implementations.Localization.Interfaces;
using BMM.Core.Translation;
using MvvmCross.Converters;

namespace BMM.Core.ValueConverters
{
    public class StreakEligibleUntilMessageVisibilityValueConverter : MvxValueConverter<DateTime, bool>
    {
        private IBMMLanguageBinder TextSource => BMMLanguageBinderLocator.TextSource;
         
        protected override bool Convert(DateTime eligibleDateTime, Type targetType, object parameter, CultureInfo culture)
        {
            if (eligibleDateTime < DateTime.UtcNow)
                return false;

            return true;
        }
    }
}