using System.Linq;
using System.Threading.Tasks;
using BMM.Api;
using BMM.Api.Abstraction;
using BMM.Core.Extensions;
using BMM.Core.GuardedActions.Base;
using BMM.Core.GuardedActions.Documents.Interfaces;
using BMM.Core.ViewModels.Base;
using MvvmCross.Base;

namespace BMM.Core.GuardedActions.Documents
{
    public class LoadMoreDocumentsAction : GuardedAction, ILoadMoreDocumentsAction
    {
        private readonly IPostprocessDocumentsAction _postprocessDocumentsAction;
        private readonly IMvxMainThreadAsyncDispatcher _mvxMainThreadAsyncDispatcher;

        public LoadMoreDocumentsAction(
            IPostprocessDocumentsAction postprocessDocumentsAction,
            IMvxMainThreadAsyncDispatcher mvxMainThreadAsyncDispatcher)
        {
            _postprocessDocumentsAction = postprocessDocumentsAction;
            _mvxMainThreadAsyncDispatcher = mvxMainThreadAsyncDispatcher;
        }

        private ILoadMoreDocumentsViewModel LoadMoreDocumentsViewModel => this.GetDataContext();

        protected override async Task Execute()
        {
            if (LoadMoreDocumentsViewModel.IsLoading || LoadMoreDocumentsViewModel.IsFullyLoaded)
                return;

            LoadMoreDocumentsViewModel.IsLoading = true;

            var documents = await LoadMoreDocumentsViewModel.LoadItems(LoadMoreDocumentsViewModel.CurrentLimit, ApiConstants.LoadMoreSize, CachePolicy.IgnoreCache);
            documents = await _postprocessDocumentsAction.ExecuteGuarded(documents);
            var docList = documents.ToList();

            LoadMoreDocumentsViewModel.CurrentLimit += ApiConstants.LoadMoreSize;

            await _mvxMainThreadAsyncDispatcher.ExecuteOnMainThreadAsync(() =>
                {
                    if (documents != null && docList.Any())
                    {
                        LoadMoreDocumentsViewModel.Documents.AddRange(docList);
                        LoadMoreDocumentsViewModel.RaisePropertyChanged(() => LoadMoreDocumentsViewModel.TrackCountString);
                    }
                    else
                    {
                        LoadMoreDocumentsViewModel.IsFullyLoaded = true;
                    }
                });
        }

        protected override Task OnFinally()
        {
            LoadMoreDocumentsViewModel.IsLoading = false;
            return base.OnFinally();
        }
    }
}