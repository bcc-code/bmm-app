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
using BMM.Core.Implementations.Storage;

namespace BMM.Core.Implementations.Podcasts
{
    public class PodcastOfflineTrackProvider : IPodcastOfflineTrackProvider
    {
        private readonly IBMMClient _client;

        public PodcastOfflineTrackProvider(IBMMClient client)
        {
            _client = client;
        }

        public async Task<IList<Track>> GetTracksSupposedToBeDownloaded()
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

        private async Task<IDictionary<int, int>> GetAutomaticallyDownloadedTracks() => AppSettings.AutomaticallyDownloadedTracks;
        public async Task<ICollection<int>> GetFollowedPodcasts() => AppSettings.LocalPodcasts;
        private async Task SaveFollowedPodcast(IEnumerable<int> podcastIds) => AppSettings.LocalPodcasts = podcastIds.ToHashSet();
    }
}