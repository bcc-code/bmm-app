using System;
using System.Globalization;
using BMM.Api.Framework;
using BMM.Api.Implementation.Models;
using BMM.Core.Implementations.FileStorage;
using BMM.Core.Implementations.UI;
using BMM.Core.ViewModels;
using BMM.UI.iOS.Constants;
using MvvmCross;
using MvvmCross.Converters;
using UIKit;

namespace BMM.UI.iOS
{
    public class TrackToTitleColorConverter: MvxValueConverter<Track, UIColor>
    {
        protected override UIColor Convert(Track currentPlayedTrack, Type targetType, object parameter, CultureInfo culture)
        {
            var wrapper = ((Func<CellWrapperViewModel<Document>>)parameter).Invoke();
            var document = wrapper.Item as Track;

            if (TrackIsCurrentlySelected(document, currentPlayedTrack))
                return AppColors.ColorPrimary;

            if (!TrackIsAvailable(document))
                return UIColor.Gray;

            if (wrapper.ViewModel is IDarkStyleOnIosViewModel)
                return UIColor.White;

            return AppColors.TrackTitleColor;

        }

        private bool TrackIsCurrentlySelected(Document track, Document currentTrack)
        {
            return currentTrack != null && track.Id.Equals(currentTrack.Id);
        }

        private bool TrackIsAvailable(Track track)
        {
            var trackIsDownloaded = Mvx.IoCProvider.Resolve<IStorageManager>().SelectedStorage.IsDownloaded(track);
            var isOnline = Mvx.IoCProvider.Resolve<IConnection>().GetStatus() == ConnectionStatus.Online;
            return isOnline || trackIsDownloaded;
        }
    }
}