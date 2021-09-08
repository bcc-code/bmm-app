using BMM.Api;
using BMM.Api.Implementation.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using BMM.Api.Abstraction;
using BMM.Core.Extensions;
using BMM.Core.GuardedActions.Documents.Interfaces;
using BMM.Core.Helpers;
using BMM.Core.Implementations.DocumentFilters;
using BMM.Core.Translation;
using MvvmCross.Commands;
using MvvmCross.IoC;
using MvvmCross.Localization;

namespace BMM.Core.ViewModels.Base
{
    public abstract class LoadMoreDocumentsViewModel : DocumentsViewModel, ILoadMoreDocumentsViewModel
    {
        private ILoadMoreDocumentsAction _loadMoreDocumentsAction;

        [MvxInject]
        public ILoadMoreDocumentsAction LoadMoreDocumentsAction
        {
            get => _loadMoreDocumentsAction;
            set
            {
                _loadMoreDocumentsAction = value;
                _loadMoreDocumentsAction.AttachDataContext(this);
            }
        }

        private IMvxAsyncCommand _loadMoreCommand;

        public IMvxAsyncCommand LoadMoreCommand => _loadMoreCommand ?? (_loadMoreCommand = new ExceptionHandlingCommand(LoadMore));

        /// <summary>
        /// An indicator if all views on that screen are fully loaded. This can easily be used within a list-view in that view, to indicate, that there is something loading.
        /// In views, having a load-more command, showing this should trigger that command. This is usually done in the LoadMoreDocumentsViewModel.
        /// </summary>
        private bool _isFullyLoaded;

        public bool IsFullyLoaded
        {
            get => _isFullyLoaded;
            set => SetProperty(ref _isFullyLoaded, value);
        }

        public virtual int CurrentLimit { get; set; } = ApiConstants.LoadMoreSize;

        public override string TrackCountString
        {
            get
            {
                string translationKeys = IsFullyLoaded
                    ? Translations.DocumentsViewModel_PluralTracks
                    : Translations.DocumentsViewModel_PluralTracksLoaded;

                return TextSource.GetText(translationKeys, Documents.Count);
            }
        }

        protected LoadMoreDocumentsViewModel(IDocumentFilter documentFilter = null)
            : base(documentFilter)
        {
            IsFullyLoaded = false;

            // Bind IsFullyLoaded also to the FilterEnabled boolean.
            PropertyChanged += (sender, e) => { if (e.PropertyName == "FilterEnabled") RaisePropertyChanged(() => IsFullyLoaded); };
        }

        public sealed override Task<IEnumerable<Document>> LoadItems(CachePolicy policy = CachePolicy.UseCacheAndRefreshOutdated)
        {
            return LoadItems(0, CurrentLimit, policy);
        }

        public abstract Task<IEnumerable<Document>> LoadItems(int startIndex, int size, CachePolicy policy);

        protected async Task LoadMore()
        {
            await LoadMoreDocumentsAction.ExecuteGuarded();
        }

        protected void ResetCurrentLimit()
        {
            CurrentLimit = ApiConstants.LoadMoreSize;
        }
    }
}