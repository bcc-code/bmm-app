using System.Globalization;
using Microsoft.Maui.Graphics.Platform;
using MvvmCross.Converters;
using Color = Android.Graphics.Color;

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

            var systemColor = Microsoft.Maui.Graphics.Color.FromArgb(value);
            return systemColor.AsColor();
        }
    }
}