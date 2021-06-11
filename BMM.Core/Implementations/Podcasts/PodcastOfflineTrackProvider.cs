using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Akavache;
using BMM.Api;
using BMM.Api.Abstraction;
using BMM.Api.Framework.Exceptions;
using BMM.Api.Implementation.Models;
using BMM.Core.Helpers;

namespace BMM.Core.Implementations.Podcasts
{
    public class PodcastOfflineTrackProvider: IPodcastOfflineTrackProvider
    {
        private readonly IBlobCache _blobCache;
        private readonly IBMMClient _client;

        public PodcastOfflineTrackProvider(IBlobCache blobCache, IBMMClient client)
        {
            _blobCache = blobCache;
            _client = client;
        }

        public async Task<IList<Track>> GetPodcastTracksSupposedToBeDownloaded()
        {
            var followedPodcasts = await GetFollowedPodcasts();
            var tracks = new List<Track>();
            foreach (var podcastId in followedPodcasts)
            {
                var automaticallyDownloadedTracks = await GetNumberOfTracksToAutomaticallyDownload(podcastId);
                if (automaticallyDownloadedTracks <= 0)
                    continue;

                try
                {
                    var podcastTracks = await _client.Podcast.GetTracks(podcastId, CachePolicy.UseCacheAndWaitForUpdates);
                    tracks.AddRange(podcastTracks.Take(automaticallyDownloadedTracks));
                }
                catch (NotFoundException)
                {
                    await SaveFollowedPodcast(followedPodcasts.Except(new []{podcastId}));
                }
            }

            return tracks;
        }

        private async Task<int> GetNumberOfTracksToAutomaticallyDownload(int podcastId)
        {
            var automaticDownloadedTracks = await GetAutomaticallyDownloadedTracks();
            if (automaticDownloadedTracks.ContainsKey(podcastId))
            {
                automaticDownloadedTracks.TryGetValue(podcastId, out var numTracks);
                return numTracks;
            }

            return GlobalConstants.DefaultNumberOfPodcastTracksToDownload;
        }

        private async Task<IDictionary<int, int>> GetAutomaticallyDownloadedTracks()
        {
            return await _blobCache.GetOrCreateObject(StorageKeys.AutomaticallyDownloadedTracks, () => new Dictionary<int, int>(), null);
        }

        public async Task<ICollection<int>> GetFollowedPodcasts()
        {
            return await _blobCache.GetOrCreateObject(StorageKeys.LocalPodcasts, () => new List<int>(), null);
        }

        private async Task SaveFollowedPodcast(IEnumerable<int> podcastIds)
        {
            await _blobCache.InsertObject(StorageKeys.LocalPodcasts, podcastIds);
        }
    }
}