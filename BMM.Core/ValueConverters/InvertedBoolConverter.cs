using System;
using System.Globalization;
using MvvmCross.Converters;

namespace BMM.Core.ValueConverters
{
    public class InvertedBoolConverter : MvxValueConverter<bool, bool>
    {
        protected override bool Convert(bool value, Type targetType, object parameter, CultureInfo culture)
        {
            return !value;
        }
    }
}