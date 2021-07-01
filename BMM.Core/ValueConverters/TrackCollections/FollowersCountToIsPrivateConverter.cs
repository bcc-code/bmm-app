using System;
using System.Globalization;
using MvvmCross.Converters;
using MvvmCross.Plugin.Visibility;
using MvvmCross.UI;

namespace BMM.Core.ValueConverters.TrackCollections
{
    public class FollowersCountToIsPrivateConverter : MvxValueConverter<int>
    {
        protected override object Convert(int value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == 0;
        }
    }
}