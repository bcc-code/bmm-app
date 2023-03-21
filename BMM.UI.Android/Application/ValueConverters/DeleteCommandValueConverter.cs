using BMM.Core.ViewModels;
using MvvmCross.Converters;
using System;
using System.Globalization;
using BMM.Core.Constants;
using MvvmCross.Commands;

namespace BMM.Core.ValueConverters
{
    public class DeleteCommandValueConverter : MvxValueConverter<CellWrapperViewModel<CultureInfoLanguage>, MvxCommand>
    {
        protected override MvxCommand Convert(CellWrapperViewModel<CultureInfoLanguage> value, Type targetType, object parameter, CultureInfo culture)
        {
            LanguageContentViewModel viewModel = value.ViewModel as LanguageContentViewModel;
            var item = value.Item;

            return new MvxCommand(() => { viewModel.DeleteCommand.Execute(item); });
        }
    }
}