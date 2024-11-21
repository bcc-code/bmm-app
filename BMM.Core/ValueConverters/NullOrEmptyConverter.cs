using System.Globalization;
using Microsoft.IdentityModel.Tokens;
using MvvmCross.Converters;

namespace BMM.Core.ValueConverters;

public class NullOrEmptyConverter : MvxValueConverter<string, bool>
{
    protected override bool Convert(string value, Type targetType, object parameter, CultureInfo culture)
    {
        return value.IsNullOrEmpty();
    }
}