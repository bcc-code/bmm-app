using BMM.Api.Abstraction;
using BMM.Api.Framework;
using BMM.Api.Implementation.Models;
using BMM.Core.Implementations.Factories.TrackCollections;
using BMM.Core.Implementations.FileStorage;
using BMM.Core.Implementations.PlaylistPersistence;
using BMM.Core.Test.Unit.ViewModels.Base;
using BMM.Core.ViewModels.MyContent;
using Moq;

namespace BMM.Core.Test.Unit.ViewModels
{
    [TestFixture]
    public class MyContentTrackCollectionsViewModelTests : BaseViewModelTests
    {
        private readonly Mock<IConnection> _connectionMock = new Mock<IConnection>();
        private readonly Mock<IOfflinePlaylistStorage> _playlistOfflineStorageMock = new Mock<IOfflinePlaylistStorage>();

        public override void SetUp()
        {
            base.SetUp();
            _connectionMock.Setup(x => x.GetStatus()).Returns(ConnectionStatus.Online);
            _playlistOfflineStorageMock.Setup(x => x.GetPlaylistIds()).ReturnsAsync(new HashSet<int>());

            Client.Setup(x => x.Playlist.GetAll(It.IsAny<CachePolicy>()))
                .Returns(Task.FromResult<IList<Playlist>>(new List<Playlist>()));
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
                new Mock<IStorageManager>().Object,
                _connectionMock.Object,
                new Mock<ITrackCollectionPOFactory>().Object,
                _playlistOfflineStorageMock.Object,
                new PlaylistPOFactory());
            newestViewModel.TextSource = TextResource.Object;

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
                mockStorage.Object,
                _connectionMock.Object,
                new Mock<ITrackCollectionPOFactory>().Object,
                _playlistOfflineStorageMock.Object,
                new PlaylistPOFactory());

            newestViewModel.TextSource = TextResource.Object;
            _connectionMock.Setup(x => x.GetStatus()).Returns(ConnectionStatus.Offline);

            // Act
            await newestViewModel.LoadItems();

            // Assert
            Assert.IsEmpty(newestViewModel.Documents);
        }
    }
}
