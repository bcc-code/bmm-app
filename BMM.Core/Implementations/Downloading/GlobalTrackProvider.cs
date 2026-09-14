using BMM.Api.Implementation.Models;
using BMM.Core.Helpers;
using BMM.Core.Implementations.Albums.Interfaces;
using BMM.Core.Implementations.Analytics;
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
        private readonly IAnalytics _analytics;
        private readonly TrackEqualityComparer _trackEqualityComparer = new();

        public GlobalTrackProvider(
            IPodcastOfflineTrackProvider podcastOfflineTrackProvider,
            ITrackCollectionOfflineTrackProvider trackCollectionOfflineTrackProvider,
            IPlaylistOfflineTrackProvider playlistOfflineTrackProvider,
            IAlbumOfflineTrackProvider albumOfflineTrackProvider,
            IAnalytics analytics)
        {
            _podcastOfflineTrackProvider = podcastOfflineTrackProvider;
            _trackCollectionOfflineTrackProvider = trackCollectionOfflineTrackProvider;
            _playlistOfflineTrackProvider = playlistOfflineTrackProvider;
            _albumOfflineTrackProvider = albumOfflineTrackProvider;
            _analytics = analytics;
        }

        public async Task<OfflineTracksResult> GetTracksSupposedToBeDownloaded()
        {
            var podcastTracksSupposedToBeDownloaded = await _podcastOfflineTrackProvider.GetTracksSupposedToBeDownloaded();
            var collectionTracksSupposedToBeDownloaded = await _trackCollectionOfflineTrackProvider.GetTracksSupposedToBeDownloaded();
            var playlistsSupposedToBeDownloaded = await _playlistOfflineTrackProvider.GetTracksSupposedToBeDownloaded();
            var albumsSupposedToBeDownloaded = await _albumOfflineTrackProvider.GetTracksSupposedToBeDownloaded();

            var allTracks = podcastTracksSupposedToBeDownloaded.Tracks
                .Union(collectionTracksSupposedToBeDownloaded.Tracks, _trackEqualityComparer)
                .Union(playlistsSupposedToBeDownloaded.Tracks, _trackEqualityComparer)
                .Union(albumsSupposedToBeDownloaded.Tracks, _trackEqualityComparer)
                .ToList();

            var tracks = allTracks
                .Where(track => !string.IsNullOrEmpty(track.Url))
                .ToList();

            LogTracksWithoutUrl(allTracks, tracks.Count);

            bool isComplete = podcastTracksSupposedToBeDownloaded.IsComplete
                              && collectionTracksSupposedToBeDownloaded.IsComplete
                              && playlistsSupposedToBeDownloaded.IsComplete
                              && albumsSupposedToBeDownloaded.IsComplete;

            return isComplete
                ? OfflineTracksResult.Complete(tracks)
                : OfflineTracksResult.Incomplete(tracks);
        }

        /// <summary>
        /// A track without a URL cannot be downloaded, so it is dropped here. That used to happen without
        /// a trace, which meant a playlist could be marked for offline use, download nothing at all, and
        /// leave no record of why. <c>media</c>, <c>files</c> and <c>url</c> are all optional in the API,
        /// so this measures how much content is affected and which kind.
        /// </summary>
        private void LogTracksWithoutUrl(IList<Track> allTracks, int downloadableCount)
        {
            int droppedCount = allTracks.Count - downloadableCount;

            if (droppedCount == 0)
                return;

            var dropped = allTracks.Where(track => string.IsNullOrEmpty(track.Url)).ToList();

            _analytics.LogEvent(
                "Tracks supposed to be downloaded have no url",
                new Dictionary<string, object>
                {
                    { "droppedCount", droppedCount },
                    { "downloadableCount", downloadableCount },
                    { "trackIds", string.Join(",", dropped.Take(20).Select(track => track.Id)) },
                    { "subtypes", string.Join(",", dropped.Select(track => track.Subtype).Distinct()) },
                    { "withoutMedia", dropped.Count(track => track.Media == null) }
                });
        }
    }
}