using System.Drawing;
using System.Globalization;
using MvvmCross.Converters;
using MvvmCross.Plugin.Color.Platforms.Android;

namespace BMM.UI.Droid.Application.ValueConverters;

public class PlatformColorConverter : MvxValueConverter<Color, Android.Graphics.Color>
{
    protected override Android.Graphics.Color Convert(Color value, Type targetType, object parameter, CultureInfo culture)
    {
        return value.ToNativeColor();
    }
}