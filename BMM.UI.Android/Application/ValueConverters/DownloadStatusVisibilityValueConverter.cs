using System;
using System.Globalization;
using BMM.Api.Implementation.Models;
using BMM.Core.Implementations.Downloading.DownloadQueue;
using BMM.Core.Implementations.FileStorage;
using BMM.Core.ViewModels;
using MvvmCross;
using MvvmCross.Converters;

namespace BMM.UI.Droid.Application.ValueConverters
{
    public abstract class DownloadStatusVisibilityValueConverter : MvxValueConverter<CellWrapperViewModel<Document>, bool>
    {
        protected override bool Convert(CellWrapperViewModel<Document> value, Type targetType, object parameter, CultureInfo culture)
        {
            var item = value.Item;

            if (!(item is Track track))
                return false;

            if (HasItRightStatus(track))
                return true;
            else
                return false;
        }

        protected abstract bool HasItRightStatus(Track track);
    }

    public class DownloadStatusDoneValueConverter : DownloadStatusVisibilityValueConverter
    {
        protected override bool HasItRightStatus(Track track)
        {
            return Mvx.IoCProvider.Resolve<IStorageManager>().SelectedStorage.IsDownloaded(track);
        }
    }

    public class DownloadStatusPendingValueConverter : DownloadStatusVisibilityValueConverter
    {
        protected override bool HasItRightStatus(Track track)
        {
            var downloadQueue = Mvx.IoCProvider.Resolve<IDownloadQueue>();
            return downloadQueue.IsQueued(track);
        }
    }

    public class DownloadStatusProgressValueConverter : DownloadStatusVisibilityValueConverter
    {
        protected override bool HasItRightStatus(Track track)
        {
            var downloadQueue = Mvx.IoCProvider.Resolve<IDownloadQueue>();
            return downloadQueue.IsDownloading(track);
        }
    }
}