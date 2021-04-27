using BMM.Api.Implementation.Models;
using BMM.Core.Implementations.Downloading.DownloadQueue;
using BMM.Core.Implementations.FileStorage;
using BMM.Core.ViewModels;
using BMM.Core.ViewModels.Base;
using MvvmCross;
using MvvmCross.Plugin.Visibility;
using MvvmCross.UI;

namespace BMM.UI.Droid.Application.ValueConverters
{
    public class OfflineAvailableTrackValueConverter : MvxBaseVisibilityValueConverter<CellWrapperViewModel<Document>>
    {
        protected override MvxVisibility Convert(CellWrapperViewModel<Document> value, object parameter, System.Globalization.CultureInfo culture)
        {
            var viewmodel = value.ViewModel;
            var item = value.Item;

            if (!(item is Track))
                return MvxVisibility.Collapsed;

            var track = (Track)item;
            if (IsOfflineAvailable(track))
                return MvxVisibility.Visible;

            if (!(viewmodel is DocumentsViewModel))
                return MvxVisibility.Collapsed;

            var downloadQueue = Mvx.IoCProvider.Resolve<IDownloadQueue>();

            if (downloadQueue.IsDownloading(track) ||
                downloadQueue.IsQueued(track))
                return MvxVisibility.Visible;

            return MvxVisibility.Collapsed;
        }

        private bool IsOfflineAvailable(Track value)
        {
            return Mvx.IoCProvider.Resolve<IStorageManager>().SelectedStorage.IsDownloaded(value);
        }
    }
}