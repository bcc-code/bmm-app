using MvvmCross.Converters;
using System;
using System.Globalization;

namespace BMM.UI.Droid.Application.ValueConverters
{
    public class FloatProgressToIntValueConverter : MvxValueConverter<float, int>
    {
        protected override int Convert(float value, Type targetType, object parameter, CultureInfo culture)
        {
            return (int)(value * (long)parameter);
        }
    }
}