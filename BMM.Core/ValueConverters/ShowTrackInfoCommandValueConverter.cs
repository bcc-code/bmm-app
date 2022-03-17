using System;
using System.Globalization;
using BMM.Api.Implementation.Models;
using BMM.Core.ViewModels;
using MvvmCross.Commands;
using MvvmCross.Converters;

namespace BMM.Core.ValueConverters
{
    public class ShowTrackInfoCommandValueConverter : MvxValueConverter<CellWrapperViewModel<Document>, IMvxCommand>
    {
        protected override IMvxCommand Convert(CellWrapperViewModel<Document> value, Type targetType, object parameter, CultureInfo culture)
        {
            var viewModel = value.ViewModel;

            var trackItem = value.Item switch
            {
                Track track => track,
                ContinueListeningTile continueListeningTile => continueListeningTile.Track,
                _ => null
            };

            return new MvxCommand(() => viewModel.ShowTrackInfoCommand.Execute(trackItem));
        }
    }
}