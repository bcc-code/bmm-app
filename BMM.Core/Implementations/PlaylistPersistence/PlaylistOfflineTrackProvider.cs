using System.Collections.Generic;
using System.Threading.Tasks;
using BMM.Api;
using BMM.Api.Abstraction;
using BMM.Api.Implementation.Models;

namespace BMM.Core.Implementations.PlaylistPersistence
{
    public class PlaylistOfflineTrackProvider : IPlaylistOfflineTrackProvider
    {
        private readonly IOfflinePlaylistStorage _playlistStorage;
        private readonly IBMMClient _client;

        public PlaylistOfflineTrackProvider(IOfflinePlaylistStorage playlistStorage, IBMMClient client)
        {
            _playlistStorage = playlistStorage;
            _client = client;
        }

        public async Task<IEnumerable<Track>> GetCollectionTracksSupposedToBeDownloaded()
        {
            var playlistIds = await _playlistStorage.GetPlaylistIds();

            var tracks = new List<Track>();
            foreach (var playlistId in playlistIds)
            {
                var playlistTracks = await _client.Playlist.GetTracks(playlistId, CachePolicy.UseCacheAndWaitForUpdates);
                tracks.AddRange(playlistTracks);
            }

            return tracks;
        }
    }
}