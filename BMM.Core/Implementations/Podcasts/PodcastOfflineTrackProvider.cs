using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Akavache;
using BMM.Api;
using BMM.Api.Abstraction;
using BMM.Api.Framework;
using BMM.Api.Framework.Exceptions;
using BMM.Api.Implementation.Models;
using BMM.Core.Helpers;
using BMM.Core.Implementations.Downloading;
using BMM.Core.Implementations.Storage;

namespace BMM.Core.Implementations.Podcasts
{
    public class PodcastOfflineTrackProvider : IPodcastOfflineTrackProvider
    {
        private readonly IBMMClient _client;
        private readonly ILogger _logger;

        public PodcastOfflineTrackProvider(IBMMClient client, ILogger logger)
        {
            _client = client;
            _logger = logger;
        }

        public async Task<OfflineTracksResult> GetTracksSupposedToBeDownloaded()
        {
            var followedPodcasts = await GetFollowedPodcasts();
            var tracks = new List<Track>();
            bool isComplete = true;

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
                catch (UnauthorizedException)
                {
                    // Has to bubble up so the user is sent to the login screen.
                    throw;
                }
                catch (Exception exception)
                {
                    // The podcast is still followed, we just couldn't read it. Saying so keeps its
                    // already downloaded episodes from being deleted as "no longer needed".
                    isComplete = false;
                    _logger.Error(GetType().Name, $"Could not read podcast {podcastId} while looking for tracks to download", exception);
                }
            }

            return isComplete
                ? OfflineTracksResult.Complete(tracks)
                : OfflineTracksResult.Incomplete(tracks);
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