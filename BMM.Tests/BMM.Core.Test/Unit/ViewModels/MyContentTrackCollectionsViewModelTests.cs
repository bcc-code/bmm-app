using System.Collections.Generic;
using System.Threading.Tasks;
using BMM.Api.Abstraction;
using BMM.Api.Framework;
using BMM.Api.Implementation.Models;
using BMM.Core.Implementations.FileStorage;
using BMM.Core.Implementations.TrackCollections;
using BMM.Core.Test.Unit.ViewModels.Base;
using BMM.Core.ViewModels.MyContent;
using Moq;
using NUnit.Framework;

namespace BMM.Core.Test.Unit.ViewModels
{
    [TestFixture]
    public class MyContentTrackCollectionsViewModelTests : BaseViewModelTests
    {
        private readonly Mock<IConnection> _connectionMock = new Mock<IConnection>();

        [SetUp]
        public void Init()
        {
            Setup();
            base.AdditionalSetup();

            _connectionMock.Setup(x => x.GetStatus()).Returns(ConnectionStatus.Online);
        }

        [Test]
        public async Task LoadItems_ShouldHandleNullValueAndDisplayEmptyDocumentIfConnectionIsOnline()
        {
            // Arrange
            Client.Setup(x => x.TrackCollection.GetAll(It.IsAny<CachePolicy>()))
                .Returns(Task.FromResult<IList<TrackCollection>>(null));

            Client.Setup(x => x.Podcast.GetAll(CachePolicy.UseCacheAndRefreshOutdated))
                .Returns(Task.FromResult<IList<Podcast>>(null));

            Client.Setup(x => x.TrackCollection.GetById(It.IsAny<int>(), It.IsAny<CachePolicy>()))
                .Returns(Task.FromResult(new TrackCollection { Tracks = new List<Track>() }));

            var newestViewModel = new DownloadedContentViewModel(
                new Mock<IOfflineTrackCollectionStorage>().Object,
                new Mock<IStorageManager>().Object,
                _connectionMock.Object);

            // Act
            await newestViewModel.LoadItems();

            // Assert
            Assert.IsEmpty(newestViewModel.Documents);
        }

        [Test]
        public async Task LoadItems_ShouldHandleNullValueAndDisplayEmptyDocumentIfConnectionIsOffline()
        {
            // Arrange
            Client.Setup(x => x.TrackCollection.GetAll(It.IsAny<CachePolicy>()))
                .Returns(Task.FromResult<IList<TrackCollection>>(null));

            Client.Setup(x => x.Podcast.GetAll(CachePolicy.UseCacheAndRefreshOutdated))
                .Returns(Task.FromResult<IList<Podcast>>(null));

            Client.Setup(x => x.TrackCollection.GetById(It.IsAny<int>(), It.IsAny<CachePolicy>()))
                .Returns(Task.FromResult(new TrackCollection { Tracks = new List<Track>() }));

            var mockFileStorage = new Mock<IFileStorage>();
            mockFileStorage.Setup(x => x.IsDownloaded(It.IsAny<Track>())).Returns(true);

            var mockStorage = new Mock<IStorageManager>();
            mockStorage.Setup(x => x.SelectedStorage).Returns(mockFileStorage.Object);


            var newestViewModel = new DownloadedContentViewModel(
                new Mock<IOfflineTrackCollectionStorage>().Object,
                mockStorage.Object,
                _connectionMock.Object);

            _connectionMock.Setup(x => x.GetStatus()).Returns(ConnectionStatus.Offline);

            // Act
            await newestViewModel.LoadItems();

            // Assert
            Assert.IsEmpty(newestViewModel.Documents);
        }
    }
}
