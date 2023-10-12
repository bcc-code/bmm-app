using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using BMM.Api.Abstraction;
using BMM.Api.Implementation.Models;
using BMM.Api.Implementation.Models.Enums;
using BMM.Core.Helpers;
using BMM.Core.Implementations.DocumentFilters;
using BMM.Core.Implementations.Factories;
using BMM.Core.Implementations.Factories.Tracks;
using BMM.Core.Models.POs.Base.Interfaces;
using BMM.Core.Models.POs.Tracks;
using BMM.Core.Translation;
using BMM.Core.ViewModels.Base;
using MvvmCross.Commands;
using MvvmCross.ViewModels;

namespace BMM.Core.ViewModels
{
    public class SearchResultsViewModel : LoadMoreDocumentsViewModel, IMvxViewModel<SearchFilter>
    {
        private readonly IDocumentsPOFactory _documentsPOFactory;
        private const string TranslationPrefix = "SearchViewModel_";

        private string _previouslyUsedSearchTerm;
        private bool _selected;
        private string _title;
        private bool _hasAnyItem;
        private bool _showNoItemsInfo;
        private string _searchTerm;
        private bool _isSearching;
        private bool _hasError;
        
        public override IEnumerable<string> PlaybackOrigin()
        {
            return new[] {SearchFilter.ToString(), _searchTerm};
        }

        public SearchResultsViewModel(
            ITrackPOFactory trackPOFactory,
            IDocumentsPOFactory documentsPOFactory,
            IDocumentFilter documentFilter = null)
            : base(
                trackPOFactory,
                documentFilter)
        {
            _documentsPOFactory = documentsPOFactory;
            ReloadCommand = new ExceptionHandlingCommand(async () => { await ReloadSearch(); });
        }

        public Action ClearFocusAction { get; set; }
        
        public IMvxAsyncCommand ReloadCommand { get; }

        public async Task Search(string searchTerm)
        {
            if (_previouslyUsedSearchTerm == searchTerm)
            {
                IsSearching = IsLoading;
                return;
            }

            Documents.Clear();
            _previouslyUsedSearchTerm = searchTerm;
            _searchTerm = searchTerm;
            await RaisePropertyChanged(nameof(NoResultsDescriptionLabel));
            await RaisePropertyChanged(nameof(SearchFailedDescriptionLabel));
            await Load();
        }

        public void Prepare(SearchFilter searchFilter)
        {
            SearchFilter = searchFilter;
            Title = TextSource[$"{TranslationPrefix}{SearchFilter}"];
        }

        protected override void AttachEvents()
        {
            base.AttachEvents();
            PropertyChanged += OnPropertyChanged;
            HandleSearchingPropertyChanged();
        }
        
        protected override void DetachEvents()
        {
            base.DetachEvents();
            PropertyChanged -= OnPropertyChanged;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(IsLoading))
                return;

            HandleSearchingPropertyChanged();
        }

        public SearchFilter SearchFilter { get; set; }

        public bool ShowNoItemsInfo
        {
            get => _showNoItemsInfo;
            set => SetProperty(ref _showNoItemsInfo, value);
        }
        
        public bool Selected
        {
            get => _selected;
            set => SetProperty(ref _selected, value);
        }
        
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }
        
        public bool HasAnyItem
        {
            get => _hasAnyItem;
            set => SetProperty(ref _hasAnyItem, value);
        }
        
        public bool IsSearching
        {
            get => _isSearching;
            set => SetProperty(ref _isSearching, value);
        }
        
        public bool HasError
        {
            get => _hasError;
            set => SetProperty(ref _hasError, value);
        }

        public string NoResultsDescriptionLabel => TextSource.GetText(Translations.SearchViewModel_NoResultsDescription, _searchTerm);
        public string SearchFailedDescriptionLabel => TextSource.GetText(Translations.SearchViewModel_SearchFailedMessage, _searchTerm);

        public override async Task<IEnumerable<IDocumentPO>> LoadItems(int startIndex, int size, CachePolicy policy)
        {
            bool isSuccess = false;
            
            try
            {
                if (string.IsNullOrEmpty(_searchTerm))
                {
                    Documents.Clear();
                    isSuccess = true;
                    return null;
                }

                var results = await Client.Search.GetAll(_searchTerm,
                    SearchFilter,
                    startIndex,
                    size);
            
                var adjustedItemsList = CreateAdjustedItemsList(results, startIndex);
            
                IsFullyLoaded = results.IsFullyLoaded;
                isSuccess = true;
                return _documentsPOFactory.Create(
                    adjustedItemsList,
                    DocumentSelectedCommand,
                    OptionCommand,
                    TrackInfoProvider);
            }
            finally
            {
                HasError = !isSuccess && !Documents.Any();
            }
        }

        private void HandleSearchingPropertyChanged()
        {
            IsSearching = IsLoading;
            HasAnyItem = Documents.Any();
            ShowNoItemsInfo = !HasAnyItem && !IsLoading && !HasError;
            RaisePropertyChanged(nameof(NoResultsDescriptionLabel));
        }

        private static IEnumerable<Document> CreateAdjustedItemsList(SearchResults searchResults, int startIndex)
        {
            var itemsList = new List<Document>();

            int index = startIndex;
            foreach (var item in searchResults.Items)
            {
                itemsList.Add(item);
                
                var existingHighlightsElement = searchResults
                    .Highlightings
                    .Where(x => x.Id.Contains(item.Id.ToString()))
                    .ToList();

                if (item is not Track trackItem || !existingHighlightsElement.Any())
                    continue;
                
                itemsList.Add(new HighlightedTextTrack(trackItem, existingHighlightsElement, index));
                index++;
            }

            return itemsList;
        }
        
        private async Task ReloadSearch()
        {
            ResetFlagsAndClear();
            await Load();
        }

        public void PrepareForSearch(string searchTerm)
        {
            if (searchTerm == _previouslyUsedSearchTerm)
                return;

            if (string.IsNullOrEmpty(searchTerm))
            {
                _previouslyUsedSearchTerm = searchTerm;
                return;
            }

            ResetFlagsAndClear();
        }

        private void ResetFlagsAndClear()
        {
            IsSearching = true;
            Documents.Clear();
            HasAnyItem = false;
            ShowNoItemsInfo = false;
            HasError = false;
        }
    }
}