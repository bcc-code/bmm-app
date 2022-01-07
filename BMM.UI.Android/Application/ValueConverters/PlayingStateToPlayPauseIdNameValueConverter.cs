using System;
using System.Globalization;
using MvvmCross.Converters;

namespace BMM.UI.Droid.Application.ValueConverters
{
    public class PlayingStateToPlayPauseIdNameValueConverter : MvxValueConverter<bool, int>
    {
        protected override int Convert(bool isPlaying, Type targetType, object parameter, CultureInfo culture)
        {
            return isPlaying
                ? Resource.Drawable.icon_pause
                : Resource.Drawable.icon_play;
        }
    }
}