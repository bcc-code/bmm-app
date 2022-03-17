using System;
using System.Globalization;
using BMM.Api.Implementation.Models;
using BMM.Api.Implementation.Models.Interfaces;
using BMM.Core.ViewModels;
using MvvmCross.Commands;
using MvvmCross.Converters;

namespace BMM.Core.ValueConverters
{
    public class PlayTrackCommandValueConverter : MvxValueConverter<CellWrapperViewModel<Document>, IMvxCommand>
    {
        protected override IMvxCommand Convert(CellWrapperViewModel<Document> value, Type targetType, object parameter, CultureInfo culture)
        {
            var track = (value.Item as ITrackHolder)?.Track;
            return track == null
                ? null
                : new MvxCommand<Document>(_ => value.ViewModel.DocumentSelectedCommand.Execute(track));
        }
    }
}