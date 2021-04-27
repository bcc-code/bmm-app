using System;
using MvvmCross.Converters;

namespace BMM.UI.iOS
{
    public class ToUppercaseConverter: MvxValueConverter<string, string>
    {
        protected override string Convert (string value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var uppercaseString = value.ToUpper();
            return uppercaseString;
        }
    }
}