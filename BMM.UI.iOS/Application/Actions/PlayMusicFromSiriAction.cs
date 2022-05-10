using System.Linq;
using System.Threading.Tasks;
using BMM.Api.Abstraction;
using BMM.Api.Implementation.Clients.Contracts;
using BMM.Api.Implementation.Constants;
using BMM.Core.Extensions;
using BMM.Core.GuardedActions.Base;
using BMM.Core.NewMediaPlayer.Abstractions;
using BMM.UI.iOS.Actions.Interfaces;
using BMM.UI.iOS.Constants;

namespace BMM.UI.iOS.Actions
{
    public class PlayMusicFromSiriAction 
        : GuardedActionWithResult<bool>,
          IPlayMusicFromSiriAction
    {
        private const int PlaylistsToTake = 8;
        private readonly IBrowseClient _browseClient;
        private readonly IPlaylistClient _playlistClient;
        private readonly IMediaPlayer _mediaPlayer;

        public PlayMusicFromSiriAction(
            IBrowseClient browseClient,
            IPlaylistClient playlistClient,
            IMediaPlayer mediaPlayer)
        {
            _browseClient = browseClient;
            _playlistClient = playlistClient;
            _mediaPlayer = mediaPlayer;
        }
        
        protected override async Task<bool> Execute()
        {
            var response = await _browseClient.GetDocuments(EndpointConstants.BrowseMusic, 0, PlaylistsToTake);

            if (response.Items == null || !response.Items.Any())
                return false;

            var randomPlaylist = response
                .Items
                .GetRandom();
            
            var tracks = await _playlistClient.GetTracks(randomPlaylist.Id, CachePolicy.IgnoreCache);

            if (tracks == null || !tracks.Any())
                return false;
            
            var mediaTracks = tracks
                .OfType<IMediaTrack>()
                .ToList();
            
            await _mediaPlayer.Play(mediaTracks, tracks.First(), SiriConstants.PlaybackOrigin);
            return true;
        }
    }
}