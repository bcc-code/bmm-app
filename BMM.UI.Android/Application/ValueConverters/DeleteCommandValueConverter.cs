using BMM.Core.ViewModels;
using MvvmCross.Converters;
using System;
using System.Globalization;
using MvvmCross.Commands;

namespace BMM.Core.ValueConverters
{
    public class DeleteCommandValueConverter : MvxValueConverter<CellWrapperViewModel<CultureInfo>, MvxCommand>
    {
        protected override MvxCommand Convert(CellWrapperViewModel<CultureInfo> value, Type targetType, object parameter, CultureInfo culture)
        {
            LanguageContentViewModel viewModel = value.ViewModel as LanguageContentViewModel;
            CultureInfo item = value.Item;

            return new MvxCommand(() => { viewModel.DeleteCommand.Execute(item); });
        }
    }
}