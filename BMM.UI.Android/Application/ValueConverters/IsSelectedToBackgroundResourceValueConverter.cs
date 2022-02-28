using System;
using System.Globalization;
using BMM.UI.Droid.Application.Constants;
using MvvmCross.Converters;

namespace BMM.UI.Droid.Application.ValueConverters
{
    public class IsSelectedToBackgroundResourceValueConverter : MvxValueConverter<bool, int>
    {
        protected override int Convert(bool isSelected, Type targetType, object parameter, CultureInfo culture)
        {
            return isSelected
                ? Resource.Drawable.button_rounded_background
                : ValueConstants.None;
        }
    }
}