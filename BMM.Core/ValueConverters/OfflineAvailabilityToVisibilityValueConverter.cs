using System.Globalization;
using BMM.Api.Implementation.Models;
using BMM.Core.Implementations.TrackCollections;
using MvvmCross;
using MvvmCross.Plugin.Visibility;
using MvvmCross.UI;

namespace BMM.Core.ValueConverters
{
    public class OfflineAvailabilityToVisibilityValueConverter: MvxBaseVisibilityValueConverter<TrackCollection>
    {
        protected override MvxVisibility Convert(TrackCollection value, object parameter, CultureInfo culture)
        {
            var offlineAvailable = Mvx.IoCProvider.Resolve<IOfflineTrackCollectionStorage>().IsOfflineAvailable(value);

            return offlineAvailable ? MvxVisibility.Visible : MvxVisibility.Collapsed;
        }
    }
}