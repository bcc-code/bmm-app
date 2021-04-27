using MvvmCross.Converters;
using System;
using System.Globalization;

namespace BMM.UI.Droid.Application.ValueConverters
{
    public class IntToStringValueConverter : MvxValueConverter<int, string>
    {
        protected override string Convert(int value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.ToString();
        }
    }
}