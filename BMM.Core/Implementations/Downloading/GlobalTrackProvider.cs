using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BMM.Api.Implementation.Models;
using BMM.Core.Helpers;
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
        private readonly TrackEqualityComparer _trackEqualityComparer = new TrackEqualityComparer();

        public GlobalTrackProvider(
            IPodcastOfflineTrackProvider podcastOfflineTrackProvider,
            ITrackCollectionOfflineTrackProvider trackCollectionOfflineTrackProvider,
            IPlaylistOfflineTrackProvider playlistOfflineTrackProvider
            )
        {
            _podcastOfflineTrackProvider = podcastOfflineTrackProvider;
            _trackCollectionOfflineTrackProvider = trackCollectionOfflineTrackProvider;
            _playlistOfflineTrackProvider = playlistOfflineTrackProvider;
        }
        public async Task<IEnumerable<Track>> GetTracksSupposedToBeDownloaded()
        {
            var podcastTracksSupposedToBeDownloaded = await _podcastOfflineTrackProvider.GetPodcastTracksSupposedToBeDownloaded();
            var collectionTracksSupposedToBeDownloaded = await _trackCollectionOfflineTrackProvider.GetCollectionTracksSupposedToBeDownloaded();
            var playlistsSupposedToBeDownloaded = await _playlistOfflineTrackProvider.GetCollectionTracksSupposedToBeDownloaded();

            return podcastTracksSupposedToBeDownloaded
                .Union(collectionTracksSupposedToBeDownloaded, _trackEqualityComparer)
                .Union(playlistsSupposedToBeDownloaded, _trackEqualityComparer);
        }

    }
}