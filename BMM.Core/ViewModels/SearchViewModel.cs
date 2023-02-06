using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Acr.UserDialogs;
using Akavache;
using BMM.Api;
using BMM.Api.Implementation.Models.Enums;
using BMM.Core.Extensions;
using BMM.Core.Helpers;
using BMM.Core.Helpers.Interfaces;
using BMM.Core.Implementations.Exceptions;
using BMM.Core.Interactions.Base;
using BMM.Core.Translation;
using BMM.Core.Utils;
using BMM.Core.ViewModels.Base;
using BMM.Core.ViewModels.Interfaces;
using MvvmCross;
using MvvmCross.Commands;
using MvvmCross.ViewModels;

namespace BMM.Core.ViewModels
{
    public class SearchViewModel : BaseViewModel, ICollectionViewModel<SearchResultsViewModel>
    {
        private readonly IMvxViewModelLoader _mvxViewModelLoader;
        private readonly IBlobCache _cache;
        private bool _isRemoveTermVisible;
        private bool _hasAnyHistoryEntry;
        private bool _searchExecuted;

        public IBmmObservableCollection<string> SearchHistory { get; }
        public IBmmInteraction RemoveFocusOnSearchInteraction { get; }
        public IBmmInteraction ResetInteraction { get; }

        public bool SearchExecuted
        {
            get => _searchExecuted;
            set => SetProperty(ref _searchExecuted, value);
        }

        private string _searchTerm;
        private SearchResultsViewModel _selectedCollectionItem;
        private bool _isHistoryVisible;

        public string SearchTerm
        {
            get => _searchTerm;
            set
            {
                SetProperty(ref _searchTerm, value);
                IsRemoveTermVisible = !string.IsNullOrEmpty(_searchTerm);
            }
        }

        public bool IsHistoryVisible
        {
            get => _isHistoryVisible;
            set => SetProperty(ref _isHistoryVisible, value);
        }
        
        public bool IsRemoveTermVisible
        {
            get => _isRemoveTermVisible;
            set => SetProperty(ref _isRemoveTermVisible, value);
        }

        public bool HasAnyHistoryEntry
        {
            get => _hasAnyHistoryEntry;
            set => SetProperty(ref _hasAnyHistoryEntry, value);
        }

        public static string SearchedString = "";
        private readonly DebounceDispatcher _debounceDispatcher;
        private readonly SemaphoreSlim _semaphoreSlim;

        public IMvxAsyncCommand SearchCommand { get; }

        public IMvxAsyncCommand<string> SearchByTermCommand { get; }

        public IMvxAsyncCommand DeleteHistoryCommand { get; }
        
        public IMvxAsyncCommand ClearCommand { get; set; }

        public SearchViewModel(
            IMvxViewModelLoader mvxViewModelLoader,
            IBlobCache cache)
        {
            _mvxViewModelLoader = mvxViewModelLoader;
            _cache = cache;
            _debounceDispatcher = new DebounceDispatcher(300);
            _semaphoreSlim = new SemaphoreSlim(1, 1);
            
            RemoveFocusOnSearchInteraction = new BmmInteraction();
            ResetInteraction = new BmmInteraction();
            SearchHistory = new BmmObservableCollection<string>();

            SearchCommand = new ExceptionHandlingCommand(
                async () =>
                {
                    if (string.IsNullOrEmpty(SearchTerm))
                    {
                        IsHistoryVisible = true;
                        return;
                    }
                    
                    await Search();
                }
            );

            SearchByTermCommand = new ExceptionHandlingCommand<string>(
                async term =>
                {
                    SearchTerm = term;
                    await Search();
                    RemoveFocusOnSearchInteraction?.Raise();
                }
            );
            
            ClearCommand = new ExceptionHandlingCommand(
                () =>
                {
                    ClearSearch();
                    SelectedCollectionItem = CollectionItems.First();
                    ResetInteraction?.Raise();
                    return Task.CompletedTask;
                });

            DeleteHistoryCommand = new ExceptionHandlingCommand(async () => await DeleteHistory());
        }

        public override IEnumerable<string> PlaybackOrigin()
        {
            return new[] {SearchedString};
        }

        public override async Task Initialize()
        {
            await base.Initialize();
            IsHistoryVisible = true;
            
            await _cache.GetOrCreateObject(
                StorageKeys.History,
                () => new List<string>()).Do(
                x =>
                {
                    SearchHistory.ReplaceWith(x);
                    HasAnyHistoryEntry = x.Any();
                }
            ).HandleExceptions<Exception, IList<string>>();
            
            var listOfVms = new List<SearchResultsViewModel>()
            {
                CreateSearchResultsViewModelFor(SearchFilter.All),
                CreateSearchResultsViewModelFor(SearchFilter.Speeches),
                CreateSearchResultsViewModelFor(SearchFilter.Music),
                CreateSearchResultsViewModelFor(SearchFilter.Contributors),
                CreateSearchResultsViewModelFor(SearchFilter.Playlists),
                CreateSearchResultsViewModelFor(SearchFilter.Podcasts),
                CreateSearchResultsViewModelFor(SearchFilter.Albums),
            };
            
            CollectionItems.AddRange(listOfVms);
            SelectedCollectionItem = CollectionItems.First();
        }

        private SearchResultsViewModel CreateSearchResultsViewModelFor(SearchFilter searchFilter)
        {
            var searchResultsViewModel = (SearchResultsViewModel)_mvxViewModelLoader.LoadViewModel(
                new MvxViewModelRequest<SearchResultsViewModel>(),
                searchFilter,
                null);
            
            searchResultsViewModel.ClearFocusAction = ClearFocusAction;
            return searchResultsViewModel;
        }

        private void ClearFocusAction()
        {
            RemoveFocusOnSearchInteraction?.Raise();
        }

        private async Task Search()
        {
            try
            {
                SearchedString = SearchTerm;

                IsHistoryVisible = false;
                SearchExecuted = true;
                ResetCurrentLimit();
                SearchTerm = SearchTerm.Trim();

                await SelectedCollectionItem.Search(SearchTerm);
                await HandleHistoryItems();
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
            }
        }

        private async Task HandleHistoryItems()
        {
            await _semaphoreSlim.Run(async () =>
            {
                if (SearchHistory.Contains(SearchTerm))
                    SearchHistory.RemoveAt(SearchHistory.IndexOf(SearchTerm));

                SearchHistory.Insert(0, SearchTerm);

                if (SearchHistory.Count > GlobalConstants.SearchHistoryCount)
                    SearchHistory.RemoveAt(SearchHistory.Count - 1);

                HasAnyHistoryEntry = SearchHistory.Any();
                await _cache.InsertObject(StorageKeys.History, SearchHistory.ToList());
            });
        }

        private async Task DeleteHistory()
        {
            var result = await Mvx.IoCProvider.Resolve<IUserDialogs>().ConfirmAsync(TextSource[Translations.SearchViewModel_DeleteConfirm]);
            if (result)
            {
                await _cache.InvalidateObject<List<string>>(StorageKeys.History);
                SearchHistory.Clear();
                HasAnyHistoryEntry = false;
            }
        }

        public void ClearSearch()
        {
            SearchExecuted = false;
            SearchTerm = string.Empty;
            RemoveFocusOnSearchInteraction?.Raise();
            IsHistoryVisible = true;
        }

        private int NextPageFromPosition { get; set; }

        protected void ResetCurrentLimit()
        {
            NextPageFromPosition = ApiConstants.LoadMoreSize;
        }

        public IBmmObservableCollection<SearchResultsViewModel> CollectionItems { get; } =
            new BmmObservableCollection<SearchResultsViewModel>();

        public SearchResultsViewModel SelectedCollectionItem
        {
            get => _selectedCollectionItem;
            set
            {
                SetProperty(ref _selectedCollectionItem, value);
                foreach (var searchResultViewModel in CollectionItems)
                    searchResultViewModel.Selected = false;

                _selectedCollectionItem.Selected = true;
                _selectedCollectionItem.PrepareForSearch(SearchTerm);

                if (!string.IsNullOrEmpty(SearchTerm))
                    _debounceDispatcher.Run(async () => await Search());
            }
        }
    }
}