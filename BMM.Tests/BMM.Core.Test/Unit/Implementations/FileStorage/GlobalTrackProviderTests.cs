using System.Collections.Generic;
using System.Threading.Tasks;
using BMM.Api.Implementation.Models;
using BMM.Core.Implementations.Albums.Interfaces;
using BMM.Core.Implementations.Downloading;
using BMM.Core.Implementations.PlaylistPersistence;
using BMM.Core.Implementations.Podcasts;
using BMM.Core.Implementations.TrackCollections;
using BMM.Core.Test.Unit.Implementations.Downloading;
using Moq;
using MvvmCross.Binding.Extensions;
using NUnit.Framework;

namespace BMM.Core.Test.Unit.Implementations.FileStorage
{
    public class GlobalTrackProviderTests
    {
        private Mock<IPodcastOfflineTrackProvider> _podcastTrackProvider;
        private Mock<ITrackCollectionOfflineTrackProvider> _trackCollectionProvider;
        private Mock<IPlaylistOfflineTrackProvider> _playlistProvider;
        private Mock<IAlbumOfflineTrackProvider> _albumProvider;
        private readonly FakeTrackFactory _fakeTrackFactory = new FakeTrackFactory();

        [SetUp]
        public void Setup()
        {
            _podcastTrackProvider = new Mock<IPodcastOfflineTrackProvider>();
            _trackCollectionProvider = new Mock<ITrackCollectionOfflineTrackProvider>();
            _playlistProvider = new Mock<IPlaylistOfflineTrackProvider>();
            _albumProvider = new Mock<IAlbumOfflineTrackProvider>();
        }

        [Test]
        public async Task TrackProvider_Should_Combine_Tracks_From_Providers()
        {
            var globalTrackProvider = new GlobalTrackProvider(_podcastTrackProvider.Object, _trackCollectionProvider.Object, _playlistProvider.Object, _albumProvider.Object);

            var podcastTracks = new List<Track>
            {
                _fakeTrackFactory.CreateTrackWithId(1),
                _fakeTrackFactory.CreateTrackWithId(2),
                _fakeTrackFactory.CreateTrackWithId(3)
            };
            var trackCollectionTracks = new List<Track>
            {
                _fakeTrackFactory.CreateTrackWithId(3),
                _fakeTrackFactory.CreateTrackWithId(4),
                _fakeTrackFactory.CreateTrackWithId(5)
            };
            var playlistTracks = new List<Track>
            {
                _fakeTrackFactory.CreateTrackWithId(5),
                _fakeTrackFactory.CreateTrackWithId(6),
                _fakeTrackFactory.CreateTrackWithId(7)
            };
            var albumTracks = new List<Track>
            {
                _fakeTrackFactory.CreateTrackWithId(8),
                _fakeTrackFactory.CreateTrackWithId(9),
                _fakeTrackFactory.CreateTrackWithId(10),
            };

            _podcastTrackProvider
                .Setup(x => x.GetTracksSupposedToBeDownloaded())
                .ReturnsAsync(podcastTracks);
            _trackCollectionProvider
                .Setup(x => x.GetTracksSupposedToBeDownloaded())
                .ReturnsAsync(trackCollectionTracks);
            _playlistProvider
                .Setup(x => x.GetTracksSupposedToBeDownloaded())
                .ReturnsAsync(playlistTracks);
            _albumProvider
                .Setup(x => x.GetTracksSupposedToBeDownloaded())
                .ReturnsAsync(albumTracks);

            var result = await globalTrackProvider.GetTracksSupposedToBeDownloaded();

            Assert.That(result.Count(), Is.EqualTo(10));
        }

        [Test]
        public async Task TrackProvider_Should_Filter_Out_Tracks_With_Null_Url()
        {
            var globalTrackProvider = new GlobalTrackProvider(_podcastTrackProvider.Object, _trackCollectionProvider.Object, _playlistProvider.Object, _albumProvider.Object);

            var tracks = new List<Track>
            {
                _fakeTrackFactory.CreateTrackWithId(1),
                new Track { Id = 2, Language = "en", Media = null },
                _fakeTrackFactory.CreateTrackWithId(3),
                new Track { Id = 4, Language = "en", Media = new List<TrackMedia>() },
            };

            _podcastTrackProvider
                .Setup(x => x.GetTracksSupposedToBeDownloaded())
                .ReturnsAsync(tracks);
            _trackCollectionProvider
                .Setup(x => x.GetTracksSupposedToBeDownloaded())
                .ReturnsAsync(new List<Track>());
            _playlistProvider
                .Setup(x => x.GetTracksSupposedToBeDownloaded())
                .ReturnsAsync(new List<Track>());
            _albumProvider
                .Setup(x => x.GetTracksSupposedToBeDownloaded())
                .ReturnsAsync(new List<Track>());

            var result = await globalTrackProvider.GetTracksSupposedToBeDownloaded();

            Assert.That(result.Count(), Is.EqualTo(2));
            Assert.That(result.All(t => !string.IsNullOrEmpty(t.Url)), Is.True);
        }
    }
}