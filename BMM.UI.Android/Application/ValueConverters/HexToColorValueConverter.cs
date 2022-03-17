using System;
using System.Globalization;
using Android.Graphics;
using MvvmCross.Converters;
using Xamarin.Essentials;

namespace BMM.UI.Droid.Application.ValueConverters
{
    public class HexToColorValueConverter : MvxValueConverter<string, Color>
    {
        protected override Color Convert(string value, Type targetType, object parameter, CultureInfo culture)
        {
            if (string.IsNullOrEmpty(value))
            {
                if (parameter is Color defaultColor)
                    return defaultColor;
                
                return Color.Transparent;
            }

            var systemColor = ColorConverters.FromHex(value);
            return systemColor.ToPlatformColor();
        }
    }
}