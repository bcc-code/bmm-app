using BMM.Api.Implementation.Models;
using BMM.Core.ViewModels;
using MvvmCross.Converters;
using System;
using System.Globalization;
using MvvmCross.Commands;

namespace BMM.Core.ValueConverters
{
    /// <summary>
    /// This converter is used for the click-command on a list, where the values are changed by the DocumentListValueConverter. You'll get an instance of Document
    /// in the command instead of the CellWrapperViewModel<Document/> instance, that is used internally in the view.
    /// </summary>
    public class DocumentSelectedCommandValueConverter : MvxValueConverter<MvxAsyncCommand<Document>, MvxAsyncCommand<CellWrapperViewModel<Document>>>
    {
        protected override MvxAsyncCommand<CellWrapperViewModel<Document>> Convert(MvxAsyncCommand<Document> value, Type targetType, object parameter, CultureInfo culture)
        {
            return new MvxAsyncCommand<CellWrapperViewModel<Document>>(async v =>
            {
                await value.ExecuteAsync(v.Item);
            });
        }
    }
}