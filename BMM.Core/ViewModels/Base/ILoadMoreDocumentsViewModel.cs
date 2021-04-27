using MvvmCross.Commands;

namespace BMM.Core.ViewModels.Base
{
    public interface ILoadMoreDocumentsViewModel
    {
        IMvxAsyncCommand LoadMoreCommand { get; }

        bool IsLoading { get; }

        bool IsFullyLoaded { get; }

        bool IsInitialized { get; }
    }
}