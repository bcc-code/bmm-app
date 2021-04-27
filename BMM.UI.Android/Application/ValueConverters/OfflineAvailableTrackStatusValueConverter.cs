using System;
using BMM.Api.Implementation.Models;
using BMM.Core.Implementations.Downloading.DownloadQueue;
using BMM.Core.Implementations.FileStorage;
using BMM.Core.ViewModels;
using MvvmCross;
using MvvmCross.Converters;

namespace BMM.UI.Droid.Application.ValueConverters
{
    public class OfflineAvailableTrackStatusValueConverter : MvxValueConverter<CellWrapperViewModel<Document>, int>
    {
        protected override int Convert(CellWrapperViewModel<Document> value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var item = value.Item;

            if (!(item is Track track))
                return 0;

            if (IsOfflineAvailable(track))
                return Resource.Drawable.icon_downloaded;

            var downloadQueue = Mvx.IoCProvider.Resolve<IDownloadQueue>();

            if (downloadQueue.IsDownloading(track))
                return Resource.Drawable.icon_loading;
            if (downloadQueue.IsQueued(track))
                return Resource.Drawable.icon_pending;

            return 0;
        }

        protected bool IsOfflineAvailable(Track value)
        {
            return Mvx.IoCProvider.Resolve<IStorageManager>().SelectedStorage.IsDownloaded(value);
        }
    }
}