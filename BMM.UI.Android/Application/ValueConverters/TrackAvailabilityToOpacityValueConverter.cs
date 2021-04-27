using System;
using System.Globalization;
using BMM.Api.Framework;
using BMM.Api.Implementation.Models;
using BMM.Core.Implementations.FileStorage;
using BMM.Core.ViewModels;
using BMM.Core.ViewModels.Base;
using MvvmCross;
using MvvmCross.Converters;

namespace BMM.UI.Droid.Application.ValueConverters
{
    public class TrackAvailabilityToOpacityValueConverter : MvxValueConverter<CellWrapperViewModel<Document>, float>
    {
        private const float FullOpacity = 1f;
        private const float HalfOpacity = .5f;

        protected override float Convert(CellWrapperViewModel<Document> value, Type targetType, object parameter, CultureInfo culture)
        {
            var item = value.Item;
            var track = (Track)item;
            var isConnectionStatusOnline = ((DocumentsViewModel)value.ViewModel).ConnectionStatus == ConnectionStatus.Online;

            if(TrackIsAvailable(track, isConnectionStatusOnline))
                return FullOpacity;

            return HalfOpacity;
        }

        private bool TrackIsAvailable(Track track, bool isConnectionStatusOnline)
        {
            var trackIsDownloaded = Mvx.IoCProvider.Resolve<IStorageManager>().SelectedStorage.IsDownloaded(track);
            return isConnectionStatusOnline || trackIsDownloaded;
        }
    }
}