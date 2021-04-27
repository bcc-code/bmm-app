using System.Collections.Generic;
using System.Threading.Tasks;
using BMM.Core.Implementations.Analytics;
using BMM.Core.Implementations.Podcasts;
using BMM.Core.Implementations.TrackCollections;

namespace BMM.Core.Implementations.FileStorage
{
    class AppContentLogger : IAppContentLogger
    {
        private readonly IPodcastOfflineTrackProvider _podcastOfflineTrackProvider;
        private readonly IOfflineTrackCollectionStorage _trackCollectionStorage;
        private readonly IStorageManager _storageManager;
        private readonly IAnalytics _analytics;

        public AppContentLogger(IPodcastOfflineTrackProvider podcastOfflineTrackProvider, IOfflineTrackCollectionStorage trackCollectionStorage, IStorageManager storageManager,
            IAnalytics analytics)
        {
            _podcastOfflineTrackProvider = podcastOfflineTrackProvider;
            _trackCollectionStorage = trackCollectionStorage;
            _storageManager = storageManager;
            _analytics = analytics;
        }

        public async Task LogAppContent(string eventName)
        {
            var followedPodcastsFromLocalStorage = await _podcastOfflineTrackProvider.GetFollowedPodcasts();
            var playlistsFromLocalStorage = _trackCollectionStorage.GetOfflineTrackCollectionIds();
            var listOfDownloadedFiles = _storageManager.SelectedStorage.IdsOfDownloadedFiles();

            _analytics.LogEvent(eventName,
                new Dictionary<string, object>
                {
                    {"FollowedPodcasts", string.Join(",", followedPodcastsFromLocalStorage)},
                    {"PlaylistsFromLocalStorage", string.Join(",", playlistsFromLocalStorage)},
                    {"FilesDownloaded", listOfDownloadedFiles.Count}
                });
        }
    }
}