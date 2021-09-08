using System;
using System.Globalization;
using BMM.Core.Helpers;
using BMM.Core.Implementations.Localization;
using BMM.Core.Implementations.Localization.Interfaces;
using MvvmCross.Converters;
using MvvmCross.Localization;

namespace BMM.UI.Droid.Application.ValueConverters
{
    public class StreakTextSourceValueConverter : MvxValueConverter<string, string>
    {
        private IBMMLanguageBinder BMMLanguageBinder => BMMLanguageBinderLocator.TextSource;

        protected override string Convert(string value, Type targetType, object parameter, CultureInfo culture)
        {
            return BMMLanguageBinder.GetText($"Streak_{value}");
        }
    }
}