using System;
using System.Globalization;
using BMM.Core.Implementations.DocumentFilters;
using BMM.Core.ViewModels.Base;
using BMM.Core.ViewModels.MyContent;
using MvvmCross.Converters;

namespace BMM.UI.iOS
{
    public class OfflineBannerVisibilityValueConverter : MvxValueConverter<DocumentsViewModel>
    {
        protected override object Convert(DocumentsViewModel viewModel, Type targetType, object parameter, CultureInfo culture)
        {
            return viewModel is DownloadedContentViewModel || viewModel.Filter is IDownloadedTracksOnlyFilter filter && filter.IsFilterActive;
        }
    }
}