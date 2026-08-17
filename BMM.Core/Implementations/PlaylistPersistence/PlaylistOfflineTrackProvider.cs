using BMM.Api.Abstraction;
using BMM.Api.Framework.Exceptions;
using BMM.Api.Implementation.Clients.Contracts;
using BMM.Api.Implementation.Models;
using BMM.Core.Implementations.Analytics;
using BMM.Core.Implementations.Downloading;

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

        public async Task<OfflineTracksResult> GetTracksSupposedToBeDownloaded()
        {
            var playlistIds = await _playlistStorage.GetPlaylistIds();

            var tracks = new List<Track>();
            bool isComplete = true;

            foreach (int playlistId in playlistIds)
            {
                var (playlistTracks, couldBeRead) = await SafeGetTracks(playlistId);
                tracks.AddRange(playlistTracks);
                isComplete &= couldBeRead;
            }

            return isComplete
                ? OfflineTracksResult.Complete(tracks)
                : OfflineTracksResult.Incomplete(tracks);
        }

        private async Task<(IEnumerable<Track> Tracks, bool CouldBeRead)> SafeGetTracks(int playlistId)
        {
            try
            {
                return (await _playlistClient.GetTracks(playlistId, CachePolicy.UseCacheAndWaitForUpdates), true);
            }
            catch (NotFoundException)
            {
                _analytics.LogEvent(Event.PlaylistNotFoundException, new Dictionary<string, object>
                {
                    {nameof(playlistId), playlistId}
                });

                // The playlist is definitively gone, so an empty list is the correct answer for it.
                return (Enumerable.Empty<Track>(), true);
            }
            catch (UnauthorizedException)
            {
                // Has to bubble up so the user is sent to the login screen.
                throw;
            }
            catch (Exception)
            {
                // The playlist might still be there, we just couldn't read it. Saying so keeps its
                // already downloaded tracks from being deleted as "no longer needed".
                return (Enumerable.Empty<Track>(), false);
            }
        }
    }
}