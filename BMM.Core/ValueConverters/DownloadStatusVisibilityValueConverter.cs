using System;
using System.Globalization;
using BMM.Api.Implementation.Models;
using BMM.Core.Implementations.Downloading.DownloadQueue;
using BMM.Core.Implementations.FileStorage;
using MvvmCross;
using MvvmCross.Converters;

namespace BMM.Core.ValueConverters
{
    public abstract class DownloadStatusVisibilityValueConverter : MvxValueConverter<Track, bool>
    {
        protected override bool Convert(Track track, Type targetType, object parameter, CultureInfo culture)
        {
            return HasItRightStatus(track);
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