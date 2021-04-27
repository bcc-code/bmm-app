using MvvmCross.Converters;
using System;
using System.Globalization;

namespace BMM.UI.iOS
{
    public class VisibilityConverter : MvxValueConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool? val = value as bool?;

            return val == true;
        }
    }
}