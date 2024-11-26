using System.Globalization;
using MvvmCross.Converters;

namespace BMM.Core.ValueConverters;

public class IsHighlightedToAlphaValueConverter : MvxValueConverter<bool, float>
{
    protected override float Convert(bool value, Type targetType, object parameter, CultureInfo culture)
    {
        return value
            ? 1f
            : 0.6f;
    }
}