using MvvmCross.Converters;
using System;
using System.Globalization;

namespace BMM.UI.iOS
{
    public class FormatConverter : MvxValueConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (parameter != null) ? string.Format((string)parameter, value) : value;
        }
    }
}