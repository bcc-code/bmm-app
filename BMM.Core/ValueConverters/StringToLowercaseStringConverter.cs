using System;
using System.Globalization;
using MvvmCross.Converters;

namespace BMM.Core.ValueConverters
{
    public class StringToLowercaseStringConverter : MvxValueConverter<string, string>
    {
        protected override string Convert(string value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.IsNullOrEmpty(value)
                ? default
                : value.ToLower();
        }
    }
}