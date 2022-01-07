using System;
using System.Globalization;
using BMM.Core.NewMediaPlayer.Abstractions;
using BMM.UI.Droid.Application.Constants;
using MvvmCross.Converters;

namespace BMM.UI.Droid.Application.ValueConverters
{
    public class RepeatModeToBackgroundResourceValueConverter : MvxValueConverter<RepeatType, int>
    {
        protected override int Convert(RepeatType repeatType, Type targetType, object parameter, CultureInfo culture)
        {
            return repeatType == RepeatType.None
                ? ValueConstants.None
                : Resource.Drawable.button_rounded_background;
        }
    }
}