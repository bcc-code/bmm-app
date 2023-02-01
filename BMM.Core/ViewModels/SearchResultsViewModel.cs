using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using BMM.Api.Abstraction;
using BMM.Api.Implementation.Models.Enums;
using BMM.Core.Implementations.DocumentFilters;
using BMM.Core.Implementations.Factories;
using BMM.Core.Implementations.Factories.Tracks;
using BMM.Core.Models.POs.Base.Interfaces;
using BMM.Core.Translation;
using BMM.Core.ViewModels.Base;
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

        public SearchResultsViewModel(
            ITrackPOFactory trackPOFactory,
            IDocumentsPOFactory documentsPOFactory,
            IDocumentFilter documentFilter = null)
            : base(
                trackPOFactory,
                documentFilter)
        {
            _documentsPOFactory = documentsPOFactory;
        }

        public Action ClearFocusAction { get; set; }

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

            IsSearching = IsLoading;
            HasAnyItem = Documents.Any();
            ShowNoItemsInfo = !HasAnyItem && !IsLoading;
            RaisePropertyChanged(nameof(NoResultsDescriptionLabel));
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

        public string NoResultsDescriptionLabel => TextSource.GetText(Translations.SearchViewModel_NoResultsDescription, _searchTerm);

        public override async Task<IEnumerable<IDocumentPO>> LoadItems(int startIndex, int size, CachePolicy policy)
        {
            if (string.IsNullOrEmpty(_searchTerm))
            {
                Documents.Clear();
                return null;
            }

            var results = await Client.Search.GetAll(_searchTerm,
                SearchFilter,
                startIndex,
                size);
            
            if (results == null)
                return Enumerable.Empty<IDocumentPO>();
            
            IsFullyLoaded = results.IsFullyLoaded;
            return _documentsPOFactory.Create(
                results.Items,
                DocumentSelectedCommand,
                OptionCommand,
                TrackInfoProvider);
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

            IsSearching = true;
            Documents.Clear();
            HasAnyItem = false;
            ShowNoItemsInfo = false;
        }
    }
}