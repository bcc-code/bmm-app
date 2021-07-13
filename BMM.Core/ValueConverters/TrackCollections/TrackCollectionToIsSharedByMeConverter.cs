using System;
using System.Globalization;
using BMM.Api.Implementation.Models;
using MvvmCross.Converters;

namespace BMM.Core.ValueConverters.TrackCollections
{
    public class TrackCollectionToIsSharedByMeConverter : MvxValueConverter<TrackCollection, bool>
    {
        protected override bool Convert(TrackCollection value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.CanEdit && value.FollowerCount != 0;
        }
    }
}