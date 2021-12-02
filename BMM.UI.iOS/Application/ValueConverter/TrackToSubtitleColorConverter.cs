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
    public class TrackToSubtitleColorConverter: MvxValueConverter<CellWrapperViewModel<Document>, UIColor>
    {
        protected override UIColor Convert(CellWrapperViewModel<Document> value, Type targetType, object parameter, CultureInfo culture)
        {
            var document = value.Item as Track;

            if (!TrackIsAvailable(document))
                return UIColor.Gray;

            if (value.ViewModel is IDarkStyleOnIosViewModel)
                return UIColor.White.ColorWithAlpha(0.8f);

            return AppColors.LabelSecondaryColor;

        }

        private bool TrackIsAvailable(Track track)
        {
            var trackIsDownloaded = Mvx.IoCProvider.Resolve<IStorageManager>().SelectedStorage.IsDownloaded(track);
            var isOnline = Mvx.IoCProvider.Resolve<IConnection>().GetStatus() == ConnectionStatus.Online;
            return isOnline || trackIsDownloaded;
        }
    }
}