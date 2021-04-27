using System;
using BMM.Api.Implementation.Models;
using BMM.Core.Implementations.Downloading.DownloadQueue;
using BMM.Core.Implementations.FileStorage;
using BMM.Core.ViewModels;
using MvvmCross;
using MvvmCross.Converters;

namespace BMM.UI.iOS
{
    public class OfflineAvailableTrackStatusConverter : MvxValueConverter
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

        public override object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var v = (CellWrapperViewModel<Document>)value;

            var item = v.Item;

            if (!(item is Track track))
            {
                return null;
            }

            if (IsOfflineAvailable(track))
            {
                return IconDownloaded;
            }

            var downloadQueue = Mvx.IoCProvider.Resolve<IDownloadQueue>();

            if (downloadQueue.IsDownloading(track))
                return IconLoading;
            if (downloadQueue.IsQueued(track))
                return IconPending;

            return null;
        }

        private bool IsOfflineAvailable(Track value)
        {
            return Mvx.IoCProvider.Resolve<IStorageManager>().SelectedStorage.IsDownloaded(value);
        }
    }
}