using System;
using System.Globalization;
using MvvmCross.Converters;
using UIKit;
using Xamarin.Essentials;

namespace BMM.UI.iOS
{
    public class HexStringToUiColorConverter: MvxValueConverter<string, UIColor>
    {
        protected override UIColor Convert(string value, Type targetType, object parameter, CultureInfo culture)
        {
            if (string.IsNullOrEmpty(value))
                return UIColor.Clear;

            var systemColor = ColorConverters.FromHex(value);

            return systemColor.ToPlatformColor();
        }
    }
}