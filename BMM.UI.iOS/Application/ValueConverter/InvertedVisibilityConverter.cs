using System.Globalization;
using MvvmCross.Converters;

namespace BMM.UI.iOS
{
    public class InvertedVisibilityConverter : MvxValueConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool? val = value as bool?;

            return val != true;
        }
    }
}