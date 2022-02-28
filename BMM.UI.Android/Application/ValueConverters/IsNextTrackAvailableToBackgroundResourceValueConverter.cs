using System;
using System.Globalization;
using BMM.Core.NewMediaPlayer.Abstractions;
using BMM.UI.Droid.Application.Constants;
using MvvmCross.Converters;

namespace BMM.UI.Droid.Application.ValueConverters
{
    public class IsNextTrackAvailableToBackgroundResourceValueConverter : MvxValueConverter<bool, int>
    {
        protected override int Convert(bool isSelected, Type targetType, object parameter, CultureInfo culture)
        {
            return isSelected
                ? Resource.Drawable.icon_next
                : Resource.Drawable.icon_next_inactive;
        }
    }
}