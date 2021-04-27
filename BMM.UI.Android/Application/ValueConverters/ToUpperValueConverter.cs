using MvvmCross.Converters;
using System;
using System.Globalization;

namespace BMM.UI.Droid.Application.ValueConverters
{
    public class ToUpperValueConverter : MvxValueConverter<string, string>
    {
        protected override string Convert(string value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.ToUpper();
        }
    }
}