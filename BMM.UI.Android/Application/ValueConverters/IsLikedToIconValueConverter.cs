using System;
using System.Globalization;
using BMM.Core.Constants;
using BMM.Core.NewMediaPlayer.Abstractions;
using BMM.UI.Droid.Application.Constants;
using MvvmCross.Converters;

namespace BMM.UI.Droid.Application.ValueConverters
{
    public class IsLikedToIconValueConverter: MvxValueConverter<bool, int>
    {
        protected override int Convert(bool isLiked, Type targetType, object parameter, CultureInfo culture)
        {
            return isLiked
                ? Resource.Drawable.icon_liked
                : Resource.Drawable.icon_unliked;
        }
    }
}