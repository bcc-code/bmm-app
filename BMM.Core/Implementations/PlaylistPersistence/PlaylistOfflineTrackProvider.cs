using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BMM.Api;
using BMM.Api.Abstraction;
using BMM.Api.Framework.Exceptions;
using BMM.Api.Implementation.Clients.Contracts;
using BMM.Api.Implementation.Models;
using BMM.Core.Implementations.Analytics;

namespace BMM.Core.Implementations.PlaylistPersistence
{
    public class PlaylistOfflineTrackProvider : IPlaylistOfflineTrackProvider
    {
        private readonly IOfflinePlaylistStorage _playlistStorage;
        private readonly IPlaylistClient _playlistClient;
        private readonly IAnalytics _analytics;

        public PlaylistOfflineTrackProvider(
            IOfflinePlaylistStorage playlistStorage,
            IPlaylistClient playlistClient,
            IAnalytics analytics)
        {
            _playlistStorage = playlistStorage;
            _playlistClient = playlistClient;
            _analytics = analytics;
        }

        public async Task<IEnumerable<Track>> GetCollectionTracksSupposedToBeDownloaded()
        {
            var playlistIds = await _playlistStorage.GetPlaylistIds();

            var tracks = new List<Track>();
            
            foreach (int playlistId in playlistIds)
            {
                var playlistTracks = await SafeGetTracks(playlistId);
                tracks.AddRange(playlistTracks);
            }

            return tracks;
        }

        private async Task<IEnumerable<Track>> SafeGetTracks(int playlistId)
        {
            try
            {
                return await _playlistClient.GetTracks(playlistId, CachePolicy.UseCacheAndWaitForUpdates);
            }
            catch (NotFoundException)
            {
                _analytics.LogEvent(Event.PlaylistNotFoundException, new Dictionary<string, object>
                {
                    {nameof(playlistId), playlistId}
                });

                return Enumerable.Empty<Track>();
            }
        }
    }
}