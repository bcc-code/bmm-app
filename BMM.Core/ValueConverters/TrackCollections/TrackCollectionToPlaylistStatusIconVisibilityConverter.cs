using System.Globalization;
using BMM.Api.Implementation.Models;
using MvvmCross.Converters;

namespace BMM.Core.ValueConverters.TrackCollections
{
    public class TrackCollectionToPlaylistStatusIconIsVisibleConverter : MvxValueConverter<TrackCollection>
    {
        protected override object Convert(TrackCollection trackCollection, Type targetType, object parameter, CultureInfo culture)
        {
            if (trackCollection == null)
                return false;

            if (trackCollection.CanEdit)
                return trackCollection.FollowerCount != 0;

            return false;
        }
    }
}