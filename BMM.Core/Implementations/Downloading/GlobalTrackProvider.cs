using BMM.Api.Implementation.Models;
using BMM.Core.Helpers;
using BMM.Core.Implementations.Albums.Interfaces;
using BMM.Core.Implementations.PlaylistPersistence;
using BMM.Core.Implementations.Podcasts;
using BMM.Core.Implementations.TrackCollections;

namespace BMM.Core.Implementations.Downloading
{
    public class GlobalTrackProvider : IGlobalTrackProvider
    {
        private readonly IPodcastOfflineTrackProvider _podcastOfflineTrackProvider;
        private readonly ITrackCollectionOfflineTrackProvider _trackCollectionOfflineTrackProvider;
        private readonly IPlaylistOfflineTrackProvider _playlistOfflineTrackProvider;
        private readonly IAlbumOfflineTrackProvider _albumOfflineTrackProvider;
        private readonly TrackEqualityComparer _trackEqualityComparer = new();

        public GlobalTrackProvider(
            IPodcastOfflineTrackProvider podcastOfflineTrackProvider,
            ITrackCollectionOfflineTrackProvider trackCollectionOfflineTrackProvider,
            IPlaylistOfflineTrackProvider playlistOfflineTrackProvider,
            IAlbumOfflineTrackProvider albumOfflineTrackProvider)
        {
            _podcastOfflineTrackProvider = podcastOfflineTrackProvider;
            _trackCollectionOfflineTrackProvider = trackCollectionOfflineTrackProvider;
            _playlistOfflineTrackProvider = playlistOfflineTrackProvider;
            _albumOfflineTrackProvider = albumOfflineTrackProvider;
        }
        
        public async Task<IEnumerable<Track>> GetTracksSupposedToBeDownloaded()
        {
            var podcastTracksSupposedToBeDownloaded = await _podcastOfflineTrackProvider.GetTracksSupposedToBeDownloaded();
            var collectionTracksSupposedToBeDownloaded = await _trackCollectionOfflineTrackProvider.GetTracksSupposedToBeDownloaded();
            var playlistsSupposedToBeDownloaded = await _playlistOfflineTrackProvider.GetTracksSupposedToBeDownloaded();
            var albumsSupposedToBeDownloaded = await _albumOfflineTrackProvider.GetTracksSupposedToBeDownloaded();

            return podcastTracksSupposedToBeDownloaded
                .Union(collectionTracksSupposedToBeDownloaded, _trackEqualityComparer)
                .Union(playlistsSupposedToBeDownloaded, _trackEqualityComparer)
                .Union(albumsSupposedToBeDownloaded, _trackEqualityComparer);
        }
    }
}