using System;
using System.Globalization;
using BMM.Api.Framework;
using BMM.Api.Implementation.Models;
using BMM.Core.Implementations.FileStorage;
using BMM.Core.ViewModels;
using BMM.Core.ViewModels.Base;
using MvvmCross;
using MvvmCross.Converters;
using UIKit;

namespace BMM.UI.iOS
{
    public class GrayOutTrackIfNecessaryConverter : MvxValueConverter<CellWrapperViewModel<Document>, UIColor>
    {
        protected override UIColor Convert(CellWrapperViewModel<Document> value, Type targetType, object parameter, CultureInfo culture)
        {
            var item = value.Item;
            var track = (Track)item;
            UIColor color = parameter as UIColor;
            var isConnectionStatusOnline = ((DocumentsViewModel)value.ViewModel).ConnectionStatus == ConnectionStatus.Online;

            if (ShouldGrayOut(track, isConnectionStatusOnline))
                return GrayOut(color);

            return color;
        }

        private UIColor GrayOut(UIColor color)
        {
            var components = color.CGColor.Components;
            return components.Length == 2 ? new UIColor(components[0], components[0], components[0], 0.5f) : new UIColor(components[0], components[1], components[2], 0.5f);
        }

        private bool ShouldGrayOut(Track track, bool isConnectionStatusOnline)
        {
            var trackIsDownloaded = Mvx.IoCProvider.Resolve<IStorageManager>().SelectedStorage.IsDownloaded(track);
            return !isConnectionStatusOnline && !trackIsDownloaded;
        }
    }
}