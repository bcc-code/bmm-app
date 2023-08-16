using System.Drawing;
using System.Globalization;
using MvvmCross.Converters;
using MvvmCross.Plugin.Color.Platforms.Ios;

namespace BMM.UI.iOS;

public class PlatformColorConverter : MvxValueConverter<Color, UIColor>
{
    protected override UIColor Convert(Color value, Type targetType, object parameter, CultureInfo culture)
    {
        return value.ToNativeColor();
    }
}