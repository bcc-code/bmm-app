using System;
using BMM.Api;
using BMM.Api.Implementation.Models;
using MvvmCross;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BMM.Api.Abstraction;
using BMM.Core.Helpers;
using BMM.Core.Implementations.DocumentFilters;
using MvvmCross.Base;
using MvvmCross.Commands;
using MvvmCross.Localization;

namespace BMM.Core.ViewModels.Base
{
    public abstract class LoadMoreDocumentsViewModel : DocumentsViewModel, ILoadMoreDocumentsViewModel
    {
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
            protected set => SetProperty(ref _isFullyLoaded, value);
        }

        public virtual int CurrentLimit { get; private set; } = ApiConstants.LoadMoreSize;

        public override string TrackCountString => new MvxLanguageBinder(GlobalConstants.GeneralNamespace, nameof(DocumentsViewModel))
            .GetText(IsFullyLoaded ? "PluralTracks" : "PluralTracksLoaded", Documents.Count);

        protected LoadMoreDocumentsViewModel(IDocumentFilter documentFilter = null, IMvxLanguageBinder languageBinder = null)
            : base(documentFilter, languageBinder)
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
            if (IsLoading || IsFullyLoaded)
            {
                return;
            }

            try
            {
                IsLoading = true;

                var documents = await LoadItems(CurrentLimit, ApiConstants.LoadMoreSize, CachePolicy.IgnoreCache);
                documents = ExcludeVideos(documents);
                documents = await EnrichDocumentsWithAdditionalData(documents);
                CurrentLimit = CurrentLimit + ApiConstants.LoadMoreSize;

                await Mvx.IoCProvider.Resolve<IMvxMainThreadAsyncDispatcher>()
                    .ExecuteOnMainThreadAsync(() =>
                    {
                        if (documents != null && documents.Any())
                        {
                            Documents.AddRange(documents);
                            RaisePropertyChanged(() => TrackCountString);
                        }

                        // TODO: Update this if the API has a fixed size for statistics
                        else
                        {
                            IsFullyLoaded = true;
                        }
                    });
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
            }
            finally
            {
                IsLoading = false;
            }
        }

        protected void ResetCurrentLimit()
        {
            CurrentLimit = ApiConstants.LoadMoreSize;
        }
    }
}