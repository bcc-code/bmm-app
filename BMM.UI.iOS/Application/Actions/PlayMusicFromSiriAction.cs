using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BMM.Api.Abstraction;
using BMM.Api.Implementation.Clients.Contracts;
using BMM.Api.Implementation.Constants;
using BMM.Core.Extensions;
using BMM.Core.GuardedActions.Base;
using BMM.Core.Implementations.Analytics;
using BMM.Core.NewMediaPlayer.Abstractions;
using BMM.UI.iOS.Actions.Interfaces;
using BMM.UI.iOS.Constants;
using BMM.UI.iOS.Utils;

namespace BMM.UI.iOS.Actions
{
    public class PlayMusicFromSiriAction 
        : GuardedActionWithResult<bool>,
          IPlayMusicFromSiriAction
    {
        private readonly ITracksClient _tracksClient;
        private readonly IPlaylistClient _playlistClient;
        private readonly IMediaPlayer _mediaPlayer;
        private readonly IAnalytics _analytics;

        public PlayMusicFromSiriAction(
            ITracksClient tracksClient,
            IPlaylistClient playlistClient,
            IMediaPlayer mediaPlayer,
            IAnalytics analytics)
        {
            _tracksClient = tracksClient;
            _playlistClient = playlistClient;
            _mediaPlayer = mediaPlayer;
            _analytics = analytics;
        }
        
        protected override async Task<bool> Execute()
        {
            _analytics.LogEvent(Event.SiriMusicPlayed);
            
            var tracks = await _tracksClient.GetRecommendations();

            if (tracks == null || !tracks.Any())
                return false;

            var mediaTracks = tracks
                .OfType<IMediaTrack>()
                .ToList();
            
            await _mediaPlayer.Play(mediaTracks, mediaTracks.First(), SiriUtils.CreatePlaybackOrigin(SiriSource.PlayMusic));
            return true;
        }
    }
}