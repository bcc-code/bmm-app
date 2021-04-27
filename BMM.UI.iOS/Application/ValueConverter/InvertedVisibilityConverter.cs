using System;
using System.Globalization;

namespace BMM.UI.iOS
{
    public class InvertedVisibilityConverter : VisibilityConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)base.Convert(value, targetType, parameter, culture) == false;
        }
    }
}