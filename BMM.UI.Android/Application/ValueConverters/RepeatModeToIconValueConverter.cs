using System;
using System.Globalization;
using BMM.Core.NewMediaPlayer.Abstractions;
using MvvmCross.Converters;

namespace BMM.UI.Droid.Application.ValueConverters
{
    public class RepeatModeToIconValueConverter: MvxValueConverter<RepeatType, int>
    {
        protected override int Convert(RepeatType repeatType, Type targetType, object parameter, CultureInfo culture)
        {
            int iconId;

            switch (repeatType)
            {
                case RepeatType.None:
                    iconId = Resource.Drawable.icon_repeat_static;
                    break;

                case RepeatType.RepeatAll:
                    iconId = Resource.Drawable.icon_repeat_active;
                    break;

                case RepeatType.RepeatOne:
                    iconId = Resource.Drawable.icon_repeat_one_active;
                    break;

                default:
                    iconId = Resource.Drawable.icon_repeat_static;
                    break;
            }

            return iconId;
        }
    }
}
