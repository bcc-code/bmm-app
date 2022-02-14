using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Acr.UserDialogs;
using Akavache;
using BMM.Api.Abstraction;
using BMM.Api.Implementation.Models;
using BMM.Core.Helpers;
using BMM.Core.Implementations.Exceptions;
using BMM.Core.Translation;
using BMM.Core.ViewModels.Base;
using MvvmCross;
using MvvmCross.Commands;
using MvvmCross.ViewModels;

namespace BMM.Core.ViewModels
{
    /// <summary>
    /// Search view model.
    ///
    /// The suggested concept for implementation is as following:
    ///
    /// Create a container, visible if SearchExecuted == FALSE, containing:
    /// * SearchHistory            (visible if ShowHistory)
    /// * "Let's start searching!" (visible if none above)
    /// This container is visible if the property SearchExecuted is FALSE.
    ///
    /// Create a second container, visible if SearchExecuted == TRUE, containing:
    /// * Suggestions  (visible if ShowSuggestions)
    /// * EmptyResults (visible if NoResults)
    /// * Results      (visible if none above)
    ///
    /// You can also (if you prefer) create a construct where a view overlays the others.
    /// </summary>
    public class SearchViewModel : LoadMoreDocumentsViewModel
    {
        public Action OnFocusLoose;

        public MvxObservableCollection<string> SearchSuggestions { get; }

        public MvxObservableCollection<string> SearchHistory { get; }

        public bool ShowSuggestions => !IsLoading && Documents.Count == 0 && SearchSuggestions.Count > 0;

        public bool ShowHistory => SearchHistory.Count > 0;

        public bool NoResults => !IsLoading && Documents.Count == 0 && SearchSuggestions.Count == 0;

        private bool _searchExecuted;

        public bool SearchExecuted
        {
            get { return _searchExecuted; }
            set { SetProperty(ref _searchExecuted, value); }
        }

        private string _searchTerm;

        public string SearchTerm
        {
            get { return _searchTerm; }
            set
            {
                SetProperty(ref _searchTerm, value);
            }
        }

        public static string SearchedString = "";

        public IMvxAsyncCommand SearchCommand { get; }

        public IMvxAsyncCommand<string> SearchByTermCommand { get; }

        public IMvxAsyncCommand DeleteHistoryCommand { get; }

        public SearchViewModel()
        {
            SearchSuggestions = new MvxObservableCollection<string>();
            SearchHistory = new MvxObservableCollection<string>();

            SearchCommand = new ExceptionHandlingCommand(
                async () => { await Search(); },
                () => !string.IsNullOrEmpty(SearchTerm)
            );

            SearchByTermCommand = new ExceptionHandlingCommand<string>(
                async term =>
                {
                    SearchTerm = term;
                    await Search();
                    OnFocusLoose?.Invoke();
                }
            );

            DeleteHistoryCommand = new ExceptionHandlingCommand(async () => await DeleteHistory());

            IsFullyLoaded = true;

            Documents.CollectionChanged += (sender, e) =>
            {
                RaisePropertyChanged(() => ShowSuggestions);
                RaisePropertyChanged(() => NoResults);
            };

            SearchSuggestions.CollectionChanged += (sender, e) =>
            {
                RaisePropertyChanged(() => ShowSuggestions);
                RaisePropertyChanged(() => NoResults);
            };

            SearchHistory.CollectionChanged += (sender, e) =>
            {
                RaisePropertyChanged(() => ShowHistory);
            };

            PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == "IsLoading")
                {
                    RaisePropertyChanged(() => ShowSuggestions);
                    RaisePropertyChanged(() => NoResults);
                }
                else if (e.PropertyName == "SearchTerm")
                {
                    SearchCommand.RaiseCanExecuteChanged();
                }
            };
        }
        
        public override IEnumerable<string> PlaybackOrigin()
        {
            return new[] {SearchedString};
        }

        protected override async Task Initialization()
        {
            await base.Initialization();
            await BlobCache.GetOrCreateObject(
                StorageKeys.History,
                () => new List<string>()).Do(
                    x =>
                    {
                        SearchHistory.ReplaceWith(x);
                    }
                ).HandleExceptions<Exception, IList<string>>();
        }

        private async Task Search()
        {
            try
            {
                SearchedString = SearchTerm;

                SearchExecuted = true;
                Documents.Clear();
                SearchSuggestions.Clear();

                ResetCurrentLimit();
                SearchTerm = SearchTerm.Trim();

                await Load();

                if (!Documents.Any())
                    await LoadSuggestions();
                else
                {
                    IsFullyLoaded = false;

                    if (SearchHistory.Contains(SearchTerm))
                        SearchHistory.RemoveAt(SearchHistory.IndexOf(SearchTerm));

                    SearchHistory.Insert(0, SearchTerm);

                    if (SearchHistory.Count > GlobalConstants.SearchHistoryCount)
                        SearchHistory.RemoveAt(SearchHistory.Count - 1);

                    await BlobCache.InsertObject<List<string>>(StorageKeys.History, SearchHistory.ToList());
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
            }
        }

        private async Task LoadSuggestions()
        {
            IsFullyLoaded = false;
            IsLoading = true;

            try
            {
                SearchSuggestions.AddRange(await Client.Search.GetSuggestions(SearchTerm));
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
            }

            IsFullyLoaded = true;
            IsLoading = false;
        }

        private async Task DeleteHistory()
        {
            var result = await Mvx.IoCProvider.Resolve<IUserDialogs>().ConfirmAsync(TextSource[Translations.SearchViewModel_DeleteConfirm]);
            if (result)
            {
                await BlobCache.InvalidateObject<List<string>>(StorageKeys.History);
                SearchHistory.Clear();
            }
        }

        public void ClearSearch()
        {
            SearchExecuted = false;
            SearchTerm = string.Empty;
            Documents.Clear();
            SearchSuggestions.Clear();
        }

        public override async Task<IEnumerable<Document>> LoadItems(int startIndex, int size, CachePolicy policy)
        {
            if (string.IsNullOrEmpty(SearchTerm))
            {
                return null;
            }

            return await Client.Search.GetAll(SearchTerm, from: startIndex, size: size);
        }
    }
}