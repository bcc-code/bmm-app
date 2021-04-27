using System;
using System.Globalization;
using MvvmCross.Converters;

namespace BMM.UI.iOS
{
    public class BoolToNfloatConverter : MvxValueConverter<bool, nfloat>
    {
        protected override nfloat Convert(bool value, Type targetType, object parameter, CultureInfo culture)
        {
            return value ? 1.0f : 0f;
        }
    }
}