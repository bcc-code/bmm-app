using System.Collections.Generic;
using System.Threading.Tasks;
using Akavache;
using BMM.Api;
using BMM.Api.Abstraction;
using BMM.Api.Implementation.Models;
using BMM.Core.Helpers;
using BMM.Core.Implementations.Analytics;
using BMM.Core.Implementations.Podcasts;
using BMM.Core.Implementations.Storage;
using Moq;
using Newtonsoft.Json;
using NSubstitute;
using NUnit.Framework;
using Plugin.Settings.Abstractions;

namespace BMM.Core.Test.Unit.Implementations.Podcasts
{
    [TestFixture]
    public class PodcastOfflineTrackProviderTests
    {
        private Mock<IBMMClient> _client;
        private IList<Track> _trackList;
        private ISettings _settingsMock;

        [SetUp]
        public void Init()
        {
            _settingsMock = Substitute.For<ISettings>();
            AppSettings.SetImplementation(_settingsMock);
            _client = new Mock<IBMMClient>();
            _trackList = GetTrackList();
        }

        [Test]
        public async Task GetOfflineTracks_ShouldReturnValidCollection()
        {
            //Arrange
            _settingsMock
                .GetValueOrDefault(nameof(AppSettings.LocalPodcasts), null)
                .Returns(JsonConvert.SerializeObject(new List<int> { 1, 2 }));

            _settingsMock
                .GetValueOrDefault(nameof(AppSettings.AutomaticallyDownloadedTracks), null)
                .Returns(JsonConvert.SerializeObject(new Dictionary<int, int> { { 1, 3 }, {2, 3} }));
            
            _client.Setup(x => x.Podcast.GetTracks(1, It.IsAny<CachePolicy>(), It.IsAny<int>(), It.IsAny<int>())).Returns(Task.FromResult(_trackList));
            _client.Setup(x => x.Podcast.GetTracks(2, It.IsAny<CachePolicy>(), It.IsAny<int>(), It.IsAny<int>())).Returns(Task.FromResult(GetTrackList(4)));

            var podcastOfflineTrackProvider = new PodcastOfflineTrackProvider(_client.Object);

            //Act
            var result = await podcastOfflineTrackProvider.GetPodcastTracksSupposedToBeDownloaded();

            //Assert
            Assert.AreEqual(6, result.Count);
        }

        [Test]
        public async Task GetOfflineTracks_ShouldReturnOneEmptyPodcastTrackList()
        {
            //Arrange
            _settingsMock
                .GetValueOrDefault(nameof(AppSettings.LocalPodcasts), null)
                .Returns(JsonConvert.SerializeObject(new List<int> { 1, 2 }));

            _settingsMock
                .GetValueOrDefault(nameof(AppSettings.AutomaticallyDownloadedTracks), null)
                .Returns(JsonConvert.SerializeObject(new Dictionary<int, int> { { 1, 3 }, {2, 0} }));
            
            _client.Setup(x => x.Podcast.GetTracks(1, It.IsAny<CachePolicy>(), It.IsAny<int>(), It.IsAny<int>())).Returns(Task.FromResult(_trackList));
            _client.Setup(x => x.Podcast.GetTracks(2, It.IsAny<CachePolicy>(), It.IsAny<int>(), It.IsAny<int>())).Returns(Task.FromResult(GetTrackList(4)));

            var podcastOfflineTrackProvider = new PodcastOfflineTrackProvider(
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
