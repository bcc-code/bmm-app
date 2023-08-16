using System.Globalization;
using BMM.UI.iOS.Extensions;
using MvvmCross.Converters;

namespace BMM.UI.iOS;

public class ImageToiOSResourceNameConverter : MvxValueConverter<string, string>
{
    protected override string Convert(string value, Type targetType, object parameter, CultureInfo culture)
    {
        return value.ToStandardIosImageName();
    }
}