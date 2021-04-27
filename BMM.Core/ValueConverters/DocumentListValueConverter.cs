using BMM.Api.Implementation.Models;
using BMM.Core.ViewModels;
using BMM.Core.ViewModels.Base;
using MvvmCross.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using MvvmCross.ViewModels;

namespace BMM.Core.ValueConverters
{
    /// <summary>
    /// This class helps to convert a list of documents to a list of documents, having a link to the ViewModel.
    /// This issue is described at https://github.com/MvvmCross/MvvmCross/issues/35
    /// If you only have a simple List instead of <see cref="MvxObservableCollection{T}"/> you should use <see cref="ListValueConverter"/>.
    /// </summary>
    public class DocumentListValueConverter : MvxValueConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var items = (MvxObservableCollection<Document>)value;
            var viewModel = (DocumentsViewModel)parameter;

            IList<CellWrapperViewModel<Document>> cellWrapperDocuments =
                items.Where(viewModel.Filter.WherePredicate)
                    .Select(x => new CellWrapperViewModel<Document>(x, viewModel))
                    .ToList();

            var documentsList = new MvxObservableCollection<CellWrapperViewModel<Document>>(cellWrapperDocuments);

            // forward the triggers, happening on the old one to the new collection.
            items.CollectionChanged += (sender, e) =>
            {
                // We always replace all items which triggers a refresh of the complete list on purpose
                // This is needed because if we only add the newest items the scroll position is changed to the bottom of the extended list. This triggers the next loadMore and therefore leads to an endless loop.

                documentsList.SwitchTo(items.Where(viewModel.Filter.WherePredicate).Select(x => new CellWrapperViewModel<Document>(x, viewModel)).ToList());
            };

            return documentsList;
        }
    }
}