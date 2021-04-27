using System.Collections.Generic;
using System.Threading.Tasks;
using BMM.Api;
using BMM.Api.Abstraction;
using BMM.Api.Framework;
using BMM.Api.Implementation.Clients.Contracts;
using BMM.Api.Implementation.Models;
using BMM.Core.Implementations.Analytics;
using BMM.Core.Implementations.Downloading;
using BMM.Core.Implementations.Exceptions;
using BMM.Core.Implementations.Security;
using BMM.Core.Implementations.TrackCollections;
using BMM.Core.Implementations.TrackCollections.Exceptions;
using Moq;
using NUnit.Framework;

namespace BMM.Core.Test.Unit.Implementations.TrackCollections
{
    [TestFixture]
    public class AddToTrackCollectionTests
    {
        private IOfflineTrackCollectionStorage _trackCollectionStorage;
        private TrackCollection _trackCollection;
        private IGlobalMediaDownloader _globalMediaDownloader;
        private Mock<ILogger> _logger;
        private Mock<IAnalytics> _analytics;
        private Mock<IUserStorage> _userStorage;
        private Mock<IExceptionHandler> _exceptionHandler;

        [SetUp]
        public void Init()
        {
            _trackCollection = new TrackCollection();
            _trackCollectionStorage = GetTrackCollectionOfflineManager();
            _globalMediaDownloader = new Mock<IGlobalMediaDownloader>().Object;
            _logger = new Mock<ILogger>();
            _analytics = new Mock<IAnalytics>();
            _userStorage = new Mock<IUserStorage>();
            _exceptionHandler = new Mock<IExceptionHandler>();
        }

        [Test]
        public async Task TrackAddedToTrackCollection_AssertSuccess()
        {
            var track = new Track { Id = 1 };
            var bmmClient = GetBmmClientThatReturnsTrackAndTrackCollection(track, true);
            var trackCollectionManager = GetTrackCollectionManager(bmmClient);
            await trackCollectionManager.AddToTrackCollection(_trackCollection, track.Id, DocumentType.Track);
        }

        [Test]
        public void NullTrackTest_AssertFail()
        {
            var track = new Track { Id = 0 };
            var bmmClient = GetBmmClientThatReturnsTrackAndTrackCollection(track, true);
            var trackCollectionManager = GetTrackCollectionManager(bmmClient);
            trackCollectionManager.AddToTrackCollection(_trackCollection, track.Id, DocumentType.Track);
            _logger.Verify(logger => logger.Error(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<NullTrackException>()), Times.AtLeastOnce);
        }

        [Test]
        public void AddedUnsupportedType_AssertExceptionThrown()
        {
            // Arrange
            var bmmClient = GetBmmClientThatReturnsAlbumAndTrackCollection(null, true);
            var trackCollectionManager = GetTrackCollectionManager(bmmClient);

            // Act & Assert
            Assert.ThrowsAsync<UnsupportedDocumentTypeException>(async () => await trackCollectionManager.AddToTrackCollection(_trackCollection, 123, DocumentType.Contributor));
        }

        private ITrackCollectionManager GetTrackCollectionManager(IBMMClient bmmClient)
        {
             return new TrackCollectionManager(bmmClient, _analytics.Object, _trackCollectionStorage, _globalMediaDownloader, _logger.Object, _exceptionHandler.Object);
        }

        private IBMMClient GetBmmClientThatReturnsAlbumAndTrackCollection(Album album, bool success)
        {
            var bmmClientMock = new Mock<IBMMClient>(MockBehavior.Strict);

            bmmClientMock
                .SetupGet(client => client.Albums)
                .Returns(GetAlbumClient(album));

            bmmClientMock
                .SetupGet(client => client.TrackCollection)
                .Returns(GetTrackCollectionClient(success));

            var bmmClient = bmmClientMock.Object;

            return bmmClient;
        }

        private IBMMClient GetBmmClientThatReturnsTrackAndTrackCollection(Track track, bool success)
        {
            var bmmClientMock = new Mock<IBMMClient>(MockBehavior.Strict);

            bmmClientMock
                .SetupGet(client => client.Tracks)
                .Returns(GetTracksClient(track));

            bmmClientMock
                .SetupGet(client => client.TrackCollection)
                .Returns(GetTrackCollectionClient(success));

            var bmmClient = bmmClientMock.Object;

            return bmmClient;
        }


        public IAlbumClient GetAlbumClient(Album album)
        {
            var albumClientMock = new Mock<IAlbumClient>(MockBehavior.Strict);
            albumClientMock
                .Setup(albumClient => albumClient.GetById(It.IsAny<int>()))
                .Returns(Task.FromResult(album));

            var albClient = albumClientMock.Object;
            return albClient;
        }

        public IOfflineTrackCollectionStorage GetTrackCollectionOfflineManager()
        {
            var trackCollectionOfflineManager = new Mock<IOfflineTrackCollectionStorage>(MockBehavior.Strict);
            trackCollectionOfflineManager
                .Setup(manager => manager.IsOfflineAvailable(_trackCollection))
                .Returns(false);

            var offlineManager = trackCollectionOfflineManager.Object;
            return offlineManager;
        }

        public ITrackCollectionClient GetTrackCollectionClient(bool success)
        {
            var trackCollectionClientMock = new Mock<ITrackCollectionClient>(MockBehavior.Strict);

            trackCollectionClientMock
                .Setup(trackCollectionClient => trackCollectionClient.AddTracksToTrackCollection(It.IsAny<int>(), It.IsAny<IList<int>>()))
                .ReturnsAsync(success);

            trackCollectionClientMock.Setup(trackCollectionClient => trackCollectionClient.GetById(_trackCollection.Id, It.IsAny<CachePolicy>()))
                .ReturnsAsync(_trackCollection);

            var collectionClient = trackCollectionClientMock.Object;
            return collectionClient;
        }

        private ITracksClient GetTracksClient(Track track)
        {
            var trackClientMock = new Mock<ITracksClient>(MockBehavior.Strict);

            trackClientMock
                .Setup(tracksClient => tracksClient.GetById(It.IsAny<int>()))
                .Returns(Task.FromResult(track));

            var trackClient = trackClientMock.Object;

            return trackClient;
        }
    }
}   
