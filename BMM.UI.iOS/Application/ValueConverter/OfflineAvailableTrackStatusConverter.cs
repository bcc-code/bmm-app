using System;
using BMM.Core.Models.POs.Tracks;
using MvvmCross.Converters;

namespace BMM.UI.iOS
{
    public class OfflineAvailableTrackStatusConverter : MvxValueConverter<TrackState, string>
    {
        public string IconDownloaded { get; protected set; }

        public string IconLoading { get; protected set; }

        public string IconPending { get; protected set; }

        public OfflineAvailableTrackStatusConverter()
        {
            IconDownloaded = "res:icon_downloaded.png";
            IconLoading = "res:icon_loading";
            IconPending = "res:icon_pending";
        }

        protected override string Convert(TrackState trackState, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (trackState.IsDownloaded)
                return IconDownloaded;
            
            if (trackState.IsDownloading)
                return IconLoading;
            
            if (trackState.IsQueued)
                return IconPending;

            return null;
        }
    }
}