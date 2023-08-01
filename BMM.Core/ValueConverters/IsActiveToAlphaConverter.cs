using System.Globalization;
using MvvmCross.Converters;

namespace BMM.Core.ValueConverters;

public class IsActiveToAlphaConverter : MvxValueConverter<bool, float>
{
    protected override float Convert(bool value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value)
            return 1f;

        return 0.3f;
    }
}