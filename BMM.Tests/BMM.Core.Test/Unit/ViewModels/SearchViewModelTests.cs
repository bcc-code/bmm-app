using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Akavache;
using BMM.Api.Implementation.Models;
using BMM.Core.GuardedActions.Documents.Interfaces;
using BMM.Core.Helpers;
using BMM.Core.Implementations.Factories;
using BMM.Core.Implementations.Factories.Tracks;
using BMM.Core.Models.POs.Base.Interfaces;
using BMM.Core.Test.Unit.ViewModels.Base;
using BMM.Core.ViewModels;
using Moq;
using NSubstitute;
using NUnit.Framework;

namespace BMM.Core.Test.Unit.ViewModels
{
    [TestFixture]
    public class SearchViewModelTests : DocumentViewModelTests
    {
        private IPostprocessDocumentsAction _postprocessDocumentsActionMock;
        private ITrackPOFactory _trackPOFactoryMock;
        private IDocumentsPOFactory _documentsPOFactoryMock;
        
        protected SearchViewModel ViewModel { get; set; }

        protected override void AdditionalSetup()
        {
            base.AdditionalSetup();

            _postprocessDocumentsActionMock = Substitute.For<IPostprocessDocumentsAction>();
            _postprocessDocumentsActionMock
                .ExecuteGuarded(Arg.Any<IEnumerable<IDocumentPO>>())
                .Returns(args => (IEnumerable<IDocumentPO>) args[0]);

            _trackPOFactoryMock = Substitute.For<ITrackPOFactory>();
            _documentsPOFactoryMock = Substitute.For<IDocumentsPOFactory>();

            ViewModel = new SearchViewModel(_trackPOFactoryMock, _documentsPOFactoryMock)
            {
                PostprocessDocumentsAction = _postprocessDocumentsActionMock
            };
        }

        [Test]
        public void Initialization()
        {
            Setup();

            Assert.IsNotNull(ViewModel.LoadMoreCommand);
            Assert.True(ViewModel.IsFullyLoaded);
            Assert.IsEmpty(ViewModel.Documents);
            Assert.That(ViewModel.SearchTerm, Is.Null.Or.Empty);
            Assert.IsNotNull(ViewModel.PlayCommand);

            Assert.IsEmpty(ViewModel.SearchSuggestions);
            Assert.IsEmpty(ViewModel.SearchHistory);
            Assert.False(ViewModel.ShowSuggestions);
            Assert.False(ViewModel.ShowHistory);
            Assert.True(ViewModel.NoResults);
            Assert.False(ViewModel.SearchExecuted);
            Assert.That(ViewModel.SearchTerm, Is.Null.Or.Empty);
            Assert.IsNotNull(ViewModel.SearchCommand);
            Assert.IsNotNull(ViewModel.SearchByTermCommand);
            Assert.IsNotNull(ViewModel.DeleteHistoryCommand);
        }

        [Test]
        public void Basic()
        {
            Setup();

            ViewModel.BlobCache = new InMemoryBlobCache();

            var propertyChangedEvents = new List<PropertyChangedEventArgs>();
            ViewModel.PropertyChanged += (sender, e) => propertyChangedEvents.Add(e);

            var task = ViewModel.Initialize();

            Assert.True(task.IsCompleted);

            Assert.IsNotNull(ViewModel.LoadMoreCommand);
            Assert.IsEmpty(ViewModel.Documents);
            Assert.That(ViewModel.SearchTerm, Is.Null.Or.Empty);
            Assert.IsNotNull(ViewModel.PlayCommand);

            Assert.IsEmpty(ViewModel.SearchSuggestions);
            Assert.IsEmpty(ViewModel.SearchHistory);
            Assert.False(ViewModel.ShowSuggestions);
            Assert.False(ViewModel.ShowHistory);
            Assert.True(ViewModel.NoResults);
            Assert.False(ViewModel.SearchExecuted);
            Assert.That(ViewModel.SearchTerm, Is.Null.Or.Empty);
            Assert.IsNotNull(ViewModel.SearchCommand);
            Assert.IsNotNull(ViewModel.SearchByTermCommand);
            Assert.IsNotNull(ViewModel.DeleteHistoryCommand);
        }

        [Test]
        public void PrefilledHistory()
        {
            Setup();

            ViewModel.BlobCache = new InMemoryBlobCache();

            ViewModel.BlobCache.InsertObject<List<string>>(StorageKeys.History, new List<string>{ "foo", "bar", "test" }).Wait();

            var propertyChangedEvents = new List<PropertyChangedEventArgs>();
            ViewModel.PropertyChanged += (sender, e) => propertyChangedEvents.Add(e);

            var task = ViewModel.Initialize();

            Assert.True(task.IsCompleted);

            Assert.IsNotNull(ViewModel.LoadMoreCommand);
            Assert.IsEmpty(ViewModel.Documents);
            Assert.That(ViewModel.SearchTerm, Is.Null.Or.Empty);
            Assert.IsNotNull(ViewModel.PlayCommand);

            Assert.IsEmpty(ViewModel.SearchSuggestions);
            Assert.AreEqual(3, ViewModel.SearchHistory.Count);
            Assert.False(ViewModel.ShowSuggestions);
            Assert.True(ViewModel.ShowHistory);
            Assert.True(ViewModel.NoResults);
            Assert.False(ViewModel.SearchExecuted);
            Assert.That(ViewModel.SearchTerm, Is.Null.Or.Empty);
            Assert.IsNotNull(ViewModel.SearchCommand);
            Assert.IsNotNull(ViewModel.SearchByTermCommand);
            Assert.IsNotNull(ViewModel.DeleteHistoryCommand);
        }

        [Test]
        public async Task LoadItems_ShouldHandleNullValueAndDisplayEmptyDocument()
        {
            // Arrange
            Setup();
            Client.Setup(x => x.Search.GetAll(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(Task.FromResult<SearchResults>(null));
            var searchViewModel = new SearchViewModel(_trackPOFactoryMock, _documentsPOFactoryMock)
            {
                SearchTerm = "test"
            };

            // Act
            await searchViewModel.LoadItems();

            // Assert
            Assert.IsEmpty(searchViewModel.Documents);
        }
    }
}