using BMM.Core.ViewModels;
using MvvmCross.Converters;
using System;
using System.Globalization;
using MvvmCross.Commands;

namespace BMM.Core.ValueConverters
{
    /// <summary>
    /// This converter is used for the click-command on a list, where the values are changed by the DocumentListValueConverter. You'll get an instance of Document
    /// in the command instead of the CellWrapperViewModel<Document> instance, that is used internally in the view.
    /// </summary>
    public class LanguageSelectedCommandValueConverter : MvxValueConverter<MvxCommand<CultureInfo>, MvxCommand<CellWrapperViewModel<CultureInfo>>>
    {
        protected override MvxCommand<CellWrapperViewModel<CultureInfo>> Convert(MvxCommand<CultureInfo> value, Type targetType, object parameter, CultureInfo culture)
        {
            return new MvxCommand<CellWrapperViewModel<CultureInfo>>((v) =>
            {
                value.Execute(v.Item);
            });
        }
    }
}