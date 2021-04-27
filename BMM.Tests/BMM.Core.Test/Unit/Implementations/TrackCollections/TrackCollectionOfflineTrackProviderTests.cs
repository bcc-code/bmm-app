using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using BMM.Api;
using BMM.Api.Abstraction;
using BMM.Api.Framework.Exceptions;
using BMM.Api.Implementation.Models;
using BMM.Core.Implementations.Analytics;
using BMM.Core.Implementations.Security;
using BMM.Core.Implementations.TrackCollections;
using Moq;
using NUnit.Framework;

namespace BMM.Core.Test.Unit.Implementations.TrackCollections
{
    [TestFixture]
    public class TrackCollectionOfflineTrackProviderTests
    {
        private Mock<IBMMClient> _client;
        private Mock<IAnalytics> _analytics;
        private Mock<IUserStorage> _userStorage;
        private Mock<IOfflineTrackCollectionStorage> _trackCollectionOfflineManager;
        private TrackCollection _trackCollection1;
        private TrackCollection _trackCollection2;

        [SetUp]
        public void Init()
        {
            _client = new Mock<IBMMClient>();
            _trackCollection1 = new TrackCollection { Tracks = GetTrackList(1) };
            _trackCollection2 = new TrackCollection { Tracks = GetTrackList(2) };
            _client.Setup(x => x.TrackCollection.GetById(1, It.IsAny<CachePolicy>())).ReturnsAsync(_trackCollection1).Verifiable();
            _client.Setup(x => x.TrackCollection.GetById(2, It.IsAny<CachePolicy>())).ReturnsAsync(_trackCollection2).Verifiable();

            _analytics = new Mock<IAnalytics>();
            _userStorage = new Mock<IUserStorage>();
            _trackCollectionOfflineManager = new Mock<IOfflineTrackCollectionStorage>();
        }

        private TrackCollectionOfflineTrackProvider GetTrackCollectionOfflineTrackProvider()
        {
            return new TrackCollectionOfflineTrackProvider(
                _client.Object,
                _trackCollectionOfflineManager.Object,
                _analytics.Object,
                _userStorage.Object);
        }

        [Test]
        public async Task GetOfflineTracks_ShouldReturnValidCollection()
        {
            // Arrange
            _trackCollectionOfflineManager.Setup(x => x.GetOfflineTrackCollectionIds()).Returns(new HashSet<int> { 1, 2 });
            var offlineTrackProvider = GetTrackCollectionOfflineTrackProvider();

            // Act
            var results = await offlineTrackProvider.GetCollectionTracksSupposedToBeDownloaded();

            // Assert
            Assert.AreEqual(4, results.Count());
            _client.Verify(x => x.TrackCollection.GetById(1, It.IsAny<CachePolicy>()), Times.Once);
            _client.Verify(x => x.TrackCollection.GetById(2, It.IsAny<CachePolicy>()), Times.Once);
        }

        [Test]
        public async Task GetOfflineTracks_ShouldCallGetByIdForEveryId()
        {
            // Arrange
            _trackCollectionOfflineManager.Setup(x => x.GetOfflineTrackCollectionIds()).Returns(new HashSet<int> {1, 2, 3, 4});
            var offlineTrackProvider = GetTrackCollectionOfflineTrackProvider();

            // Act
            var results = await offlineTrackProvider.GetCollectionTracksSupposedToBeDownloaded();

            // Assert
            Assert.AreEqual(4, results.Count());
            _client.Verify(x => x.TrackCollection.GetById(1, It.IsAny<CachePolicy>()), Times.Once);
            _client.Verify(x => x.TrackCollection.GetById(2, It.IsAny<CachePolicy>()), Times.Once);
            _client.Verify(x => x.TrackCollection.GetById(It.IsAny<int>(), It.IsAny<CachePolicy>()), Times.Exactly(4));
        }

        [Test]
        public async Task GetOfflineTracks_ShouldReturnTracksWithRequireIds()
        {
            // Arrange
            _trackCollectionOfflineManager.Setup(x => x.GetOfflineTrackCollectionIds()).Returns(new HashSet<int> { 1, 3, 4 });
            _client.Setup(x => x.TrackCollection.GetById(5, It.IsAny<CachePolicy>())).ReturnsAsync((new TrackCollection { Tracks = GetTrackList(5) })).Verifiable();
            _client.Setup(x => x.TrackCollection.GetById(5, It.IsAny<CachePolicy>())).ReturnsAsync((new TrackCollection { Tracks = GetTrackList(5) })).Verifiable();

            var offlineTrackProvider = GetTrackCollectionOfflineTrackProvider();

            // Act
            var results = (await offlineTrackProvider.GetCollectionTracksSupposedToBeDownloaded()).ToList();

            // Assert
            Assert.AreEqual(2, results.Count);
            Assert.AreEqual(1, results.First().Id);
            _client.Verify(x => x.TrackCollection.GetById(1, It.IsAny<CachePolicy>()), Times.Once);
            _client.Verify(x => x.TrackCollection.GetById(It.IsAny<int>(), It.IsAny<CachePolicy>()), Times.Exactly(3));
        }

        [Test]
        public async Task GetOfflineTracks_ShouldRemoveNotFoundPlaylistFromLocalStorage()
        {
            // Arrange
            _client.Setup(x => x.TrackCollection.GetById(3, It.IsAny<CachePolicy>())).Throws(new NotFoundException(new HttpRequestMessage(), new HttpResponseMessage()));
            _userStorage.Setup(x => x.GetUser()).Returns(new User { Username = "ola.normann", PersonId = 42526 });
            _trackCollectionOfflineManager.Setup(x => x.GetOfflineTrackCollectionIds()).Returns(new Collection<int> {1, 2, 3});
            var offlineTrackProvider = new TrackCollectionOfflineTrackProvider(_client.Object, _trackCollectionOfflineManager.Object, _analytics.Object, _userStorage.Object);

            // Act
            var response = await offlineTrackProvider.GetCollectionTracksSupposedToBeDownloaded();

            // Assert
            _trackCollectionOfflineManager.Verify(x => x.Remove(3));
            Assert.AreEqual(_trackCollection1.TrackCount + _trackCollection2.TrackCount, response.Count());
        }

        private List<Track> GetTrackList(int trackId = 1)
        {
            return new List<Track>
            {
                new Track {Id = trackId, Language = "nb", Media = new List<TrackMedia> {new TrackMedia()}},
                new Track {Id = trackId, Language = "nb", Media = new List<TrackMedia> {new TrackMedia()}}
            };
        }
    }
}
