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
    public class OptionButtonCommandValueConverter : MvxValueConverter<CellWrapperViewModel<Document>, MvxAsyncCommand>
    {
        protected override MvxAsyncCommand Convert(CellWrapperViewModel<Document> value, Type targetType, object parameter, CultureInfo culture)
        {
            BaseViewModel viewModel = value.ViewModel;
            Document item = value.Item;

            return new ExceptionHandlingCommand(async () => { await viewModel.OptionCommand.ExecuteAsync(item); });
        }
    }
}