using System;
using System.Globalization;
using BMM.Core.Helpers;
using MvvmCross.Converters;
using MvvmCross.Localization;

namespace BMM.UI.Droid.Application.ValueConverters
{
    public class StreakTextSourceValueConverter : MvxValueConverter<string, string>
    {
        private static IMvxLanguageBinder TextSource => new MvxLanguageBinder(GlobalConstants.GeneralNamespace, "Streak");
        protected override string Convert(string value, Type targetType, object parameter, CultureInfo culture)
        {
            return TextSource.GetText(value);
        }
    }
}