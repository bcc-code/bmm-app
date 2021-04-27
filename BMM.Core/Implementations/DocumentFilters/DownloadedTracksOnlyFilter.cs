using BMM.Api.Implementation.Models;
using BMM.Core.Implementations.FileStorage;
using BMM.Core.ViewModels.MyContent;

namespace BMM.Core.Implementations.DocumentFilters
{
    public interface IDownloadedTracksOnlyFilter : IDocumentFilter
    {
        bool IsFilterActive { get; }
    }

    public class DownloadedTracksOnlyFilter : IDownloadedTracksOnlyFilter
    {
        private readonly IStorageManager _storageManager;

        public bool IsFilterActive { get; }

        public DownloadedTracksOnlyFilter(IStorageManager storageManager, IViewModelAwareViewPresenter viewPresenter)
        {
            _storageManager = storageManager;

            IsFilterActive = viewPresenter.IsViewModelInStack<DownloadedContentViewModel>() ||
                             viewPresenter.IsViewModelShown<DownloadedContentViewModel>();
        }

        public bool WherePredicate(Document document)
        {
            if (IsFilterActive && document is Track track)
            {
                return _storageManager.SelectedStorage.IsDownloaded(track);
            }

            return true;
        }
    }
}