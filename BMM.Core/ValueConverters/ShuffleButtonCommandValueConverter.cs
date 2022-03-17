using BMM.Api.Implementation.Models;
using BMM.Core.ViewModels;
using BMM.Core.ViewModels.Base;
using MvvmCross.Converters;
using System;
using System.Globalization;
using BMM.Core.Helpers;
using MvvmCross.Commands;

namespace BMM.Core.ValueConverters
{
    public class ShuffleButtonCommandValueConverter : MvxValueConverter<CellWrapperViewModel<Document>, MvxAsyncCommand>
    {
        protected override MvxAsyncCommand Convert(CellWrapperViewModel<Document> value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value.ViewModel is not ExploreNewestViewModel viewModel)
                return default;
            
            if (value.Item is not ContinueListeningTile continueListeningTile)
                return null;
            
            return new ExceptionHandlingCommand(async () => { await viewModel.ShuffleButtonCommand.ExecuteAsync(continueListeningTile); });
        }
    }
}