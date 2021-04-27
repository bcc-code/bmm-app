using System.Collections.Generic;
using System.Threading.Tasks;
using Akavache;
using BMM.Api;
using BMM.Api.Abstraction;
using BMM.Api.Implementation.Models;
using BMM.Core.Helpers;
using BMM.Core.Implementations.Analytics;
using BMM.Core.Implementations.Podcasts;
using Moq;
using NUnit.Framework;

namespace BMM.Core.Test.Unit.Implementations.Podcasts
{
    [TestFixture]
    public class PodcastOfflineTrackProviderTests
    {

        private Mock<IBMMClient> _client;
        private Mock<IAnalytics> _analytics;
        private IBlobCache _inMemoryCache;
        private IList<Track> _trackList;

        [SetUp]
        public void Init()
        {
            _client = new Mock<IBMMClient>();
            _analytics = new Mock<IAnalytics>();
            _inMemoryCache = new InMemoryBlobCache();

            _trackList = GetTrackList();
        }

        [Test]
        public async Task GetOfflineTracks_ShouldReturnValidCollection()
        {
            //Arrange
            _inMemoryCache.InsertObject(StorageKeys.LocalPodcasts, new List<int> { 1, 2}, null);
            _inMemoryCache.InsertObject(StorageKeys.AutomaticallyDownloadedTracks, new Dictionary<int, int> { { 1, 3 }, {2, 3} }, null);
            _client.Setup(x => x.Podcast.GetTracks(1, It.IsAny<CachePolicy>(), It.IsAny<int>(), It.IsAny<int>())).Returns(Task.FromResult(_trackList));
            _client.Setup(x => x.Podcast.GetTracks(2, It.IsAny<CachePolicy>(), It.IsAny<int>(), It.IsAny<int>())).Returns(Task.FromResult(GetTrackList(4)));

            var podcastOfflineTrackProvider = new PodcastOfflineTrackProvider(
                _inMemoryCache,
                _client.Object);

            //Act
            var result = await podcastOfflineTrackProvider.GetPodcastTracksSupposedToBeDownloaded();

            //Assert
            Assert.AreEqual(6, result.Count);
        }

        [Test]
        public async Task GetOfflineTracks_ShouldReturnOneEmptyPodcastTrackList()
        {
            //Arrange
            _inMemoryCache.InsertObject(StorageKeys.LocalPodcasts, new List<int> { 1, 2 }, null);
            _inMemoryCache.InsertObject(StorageKeys.AutomaticallyDownloadedTracks, new Dictionary<int, int> { { 1, 3 }, { 2, 0 } }, null);
            _client.Setup(x => x.Podcast.GetTracks(1, It.IsAny<CachePolicy>(), It.IsAny<int>(), It.IsAny<int>())).Returns(Task.FromResult(_trackList));
            _client.Setup(x => x.Podcast.GetTracks(2, It.IsAny<CachePolicy>(), It.IsAny<int>(), It.IsAny<int>())).Returns(Task.FromResult(GetTrackList(4)));

            var podcastOfflineTrackProvider = new PodcastOfflineTrackProvider(
                _inMemoryCache,
                _client.Object);

            //Act
            var result = await podcastOfflineTrackProvider.GetPodcastTracksSupposedToBeDownloaded();

            //Assert
            Assert.AreEqual(3, result.Count);
        }

        private IList<Track> GetTrackList(int idStartWith = 1)
        {
            return new List<Track>
            {
                new Track {Id = idStartWith, Language = "nb", Media = new List<TrackMedia> {new TrackMedia()}},
                new Track {Id = idStartWith + 1, Language = "nb", Media = new List<TrackMedia> {new TrackMedia()}},
                new Track {Id = idStartWith + 2, Language = "nb", Media = new List<TrackMedia> {new TrackMedia()}}
            };
        }
    }
}
