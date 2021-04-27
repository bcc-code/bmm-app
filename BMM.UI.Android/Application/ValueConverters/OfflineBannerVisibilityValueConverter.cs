using System.Globalization;
using BMM.Core.Implementations.DocumentFilters;
using BMM.Core.ViewModels.Base;
using BMM.Core.ViewModels.MyContent;
using MvvmCross.Plugin.Visibility;
using MvvmCross.UI;

namespace BMM.UI.Droid.Application.ValueConverters
{
    public class OfflineBannerVisibilityValueConverter : MvxBaseVisibilityValueConverter<DocumentsViewModel>
    {
        protected override MvxVisibility Convert(DocumentsViewModel viewModel, object parameter, CultureInfo culture)
        {
            if (viewModel is DownloadedContentViewModel || viewModel.Filter is IDownloadedTracksOnlyFilter filter && filter.IsFilterActive)
            {
                return MvxVisibility.Visible;
            }

            return MvxVisibility.Collapsed;
        }
    }
}