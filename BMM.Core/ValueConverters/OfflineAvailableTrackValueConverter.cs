using BMM.Core.Models.POs.Tracks;
using MvvmCross.Plugin.Visibility;
using MvvmCross.UI;

namespace BMM.Core.ValueConverters
{
    public class OfflineAvailableTrackValueConverter : MvxBaseVisibilityValueConverter<TrackState>
    {
        protected override MvxVisibility Convert(TrackState trackState, object parameter, System.Globalization.CultureInfo culture)
        {
            if (trackState.IsDownloading || trackState.IsDownloaded || trackState.IsQueued || trackState.IsListened)
                return MvxVisibility.Visible;

            return MvxVisibility.Collapsed;
        }
    }
}