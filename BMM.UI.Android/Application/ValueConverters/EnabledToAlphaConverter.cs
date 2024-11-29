using System.Globalization;
using MvvmCross.Converters;

namespace BMM.UI.Droid.Application.ValueConverters;

public class EnabledToAlphaConverter : MvxValueConverter<bool, float>
{
    private const float EnabledAlpha = 1f;
    private const float DisabledAlpha = 0.5f;

    protected override float Convert(bool value, Type targetType, object parameter, CultureInfo culture)
    {
        return value 
            ? EnabledAlpha
            : DisabledAlpha;
    }
}