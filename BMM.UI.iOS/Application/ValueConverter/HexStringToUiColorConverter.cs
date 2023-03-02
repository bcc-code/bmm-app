using System.Globalization;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Graphics.Platform;
using MvvmCross.Converters;

namespace BMM.UI.iOS
{
    public class HexStringToUiColorConverter : MvxValueConverter<string, UIColor>
    {
        protected override UIColor Convert(string value, Type targetType, object parameter, CultureInfo culture)
        {
            if (string.IsNullOrEmpty(value))
            {
                if (parameter is UIColor defaultColor)
                    return defaultColor;
                
                return UIColor.Clear;
            }

            var systemColor = Color.FromArgb(value);
            return systemColor.AsUIColor();
        }
    }
}