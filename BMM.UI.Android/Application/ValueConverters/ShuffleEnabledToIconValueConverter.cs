using System;
using System.Globalization;
using BMM.Core.Constants;
using BMM.Core.NewMediaPlayer.Abstractions;
using BMM.UI.Droid.Application.Constants;
using MvvmCross.Converters;

namespace BMM.UI.Droid.Application.ValueConverters
{
    public class ShuffleEnabledToIconValueConverter: MvxValueConverter<bool, int>
    {
        protected override int Convert(bool shuffleEnabled, Type targetType, object parameter, CultureInfo culture)
        {
            if (shuffleEnabled)
                return Resource.Drawable.icon_shuffle_selected;

            return Resource.Drawable.icon_shuffle;
        }
    }
}