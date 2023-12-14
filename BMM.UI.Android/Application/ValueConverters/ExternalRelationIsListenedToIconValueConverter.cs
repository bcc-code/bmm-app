using System.Globalization;
using MvvmCross.Converters;

namespace BMM.UI.Droid.Application.ValueConverters;

public class ExternalRelationIsListenedToIconValueConverter : MvxValueConverter<bool, int>
{
    protected override int Convert(bool isListened, Type targetType, object parameter, CultureInfo culture)
    {
        return isListened
            ? Resource.Drawable.icon_checkmark
            : Resource.Drawable.icon_play;
    }
}