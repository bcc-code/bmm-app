using System;
using System.Globalization;
using MvvmCross.Converters;

namespace BMM.Core.ValueConverters
{
    public class HasValueToVisibleValueConverter : MvxValueConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture) => value != null;
    }
}