using BMM.Api.Implementation.Models;
using MvvmCross.Converters;
using System;
using System.Globalization;
using BMM.Core.ViewModels;
using BMM.Core.ViewModels.Base;

namespace BMM.Core.ValueConverters
{
    public class DocumentToTitleValueConverter : MvxValueConverter<CellWrapperViewModel<Document>, string>
    {
        protected override string Convert(CellWrapperViewModel<Document> document, Type targetType, object parameter, CultureInfo culture)
        {
            var track = (Track)document.Item;
            var documentViewModel = (DocumentsViewModel)document.ViewModel;
            var trackInfo = documentViewModel.TrackInfoProvider.GetTrackInformation(track, culture);
            return trackInfo.Label;
        }
    }
}