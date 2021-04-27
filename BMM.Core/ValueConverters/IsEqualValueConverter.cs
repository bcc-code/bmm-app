using MvvmCross.Converters;
using System;
using System.Globalization;

namespace BMM.Core
{
    public class IsEqualValueConverter : MvxValueConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Equals(value, parameter);
        }
    }
}