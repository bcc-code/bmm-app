using System;
using System.Globalization;
using BMM.Api.Implementation.Models;
using BMM.Core.ViewModels;
using BMM.Core.ViewModels.Base;
using MvvmCross.Converters;

namespace BMM.Core.ValueConverters
{
    public class DocumentToMetaValueConverter : MvxValueConverter<CellWrapperViewModel<Document>, string>
    {
        protected override string Convert(CellWrapperViewModel<Document> document, Type targetType, object parameter, CultureInfo culture)
        {
            var track = (Track)document.Item;
            var documentViewModel = (DocumentsViewModel)document.ViewModel;
            var trackInfo = documentViewModel.TrackInfoProvider.GetTrackInformation(track, culture);
            return trackInfo.Meta;
        }
    }
}