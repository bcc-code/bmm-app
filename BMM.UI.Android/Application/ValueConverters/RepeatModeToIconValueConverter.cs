using System;
using System.Globalization;
using BMM.Core.Constants;
using BMM.Core.NewMediaPlayer.Abstractions;
using BMM.UI.Droid.Application.Constants;
using MvvmCross.Converters;

namespace BMM.UI.Droid.Application.ValueConverters
{
    public class RepeatModeToIconValueConverter: MvxValueConverter<RepeatType, int>
    {
        protected override int Convert(RepeatType repeatType, Type targetType, object parameter, CultureInfo culture)
        {
            switch (repeatType)
            {
                case RepeatType.None:
                    return Resource.Drawable.icon_repeat;
                case RepeatType.RepeatAll:
                    return Resource.Drawable.icon_repeat_selected;
                case RepeatType.RepeatOne:
                    return Resource.Drawable.icon_repeat_single;
                default:
                    return ValueConstants.None;
            }
        }
    }
}