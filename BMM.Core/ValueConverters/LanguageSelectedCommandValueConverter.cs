using BMM.Core.ViewModels;
using MvvmCross.Converters;
using System;
using System.Globalization;
using BMM.Core.Constants;
using MvvmCross.Commands;

namespace BMM.Core.ValueConverters
{
    /// <summary>
    /// This converter is used for the click-command on a list, where the values are changed by the DocumentListValueConverter. You'll get an instance of Document
    /// in the command instead of the CellWrapperViewModel<Document> instance, that is used internally in the view.
    /// </summary>
    public class LanguageSelectedCommandValueConverter : MvxValueConverter<MvxCommand<CultureInfoLanguage>, MvxCommand<CellWrapperViewModel<CultureInfoLanguage>>>
    {
        protected override MvxCommand<CellWrapperViewModel<CultureInfoLanguage>> Convert(MvxCommand<CultureInfoLanguage> value, Type targetType, object parameter, CultureInfo culture)
        {
            return new MvxCommand<CellWrapperViewModel<CultureInfoLanguage>>((v) =>
            {
                value.Execute(v.Item);
            });
        }
    }
}