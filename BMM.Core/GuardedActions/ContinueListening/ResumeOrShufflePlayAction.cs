using System.Linq;
using System.Threading.Tasks;
using BMM.Api.Abstraction;
using BMM.Api.Implementation.Models;
using BMM.Core.Extensions;
using BMM.Core.GuardedActions.Base;
using BMM.Core.GuardedActions.ContinueListening.Interfaces;
using BMM.Core.Implementations.Analytics;
using BMM.Core.Models.POs.Tracks.Interfaces;
using BMM.Core.NewMediaPlayer.Abstractions;
using BMM.Core.ViewModels.Interfaces;

namespace BMM.Core.GuardedActions.ContinueListening
{
    public class ResumeOrShufflePlayAction : GuardedAction, IResumeOrShufflePlayAction
    {
        private readonly IMediaPlayer _mediaPlayer;
        private readonly IAnalytics _analytics;

        public ResumeOrShufflePlayAction(
            IMediaPlayer mediaPlayer,
            IAnalytics analytics)
        {
            _mediaPlayer = mediaPlayer;
            _analytics = analytics;
        }
        
        private IAlbumViewModel AlbumViewModel => this.GetDataContext();
        
        protected override async Task Execute()
        {
            var tracks = AlbumViewModel
                .Documents
                .OfType<ITrackPO>()
                .Select(t => (IMediaTrack)t.Track)
                .ToList();

            if (!AlbumViewModel.Album.LatestTrackId.HasValue)
            {
                await _mediaPlayer.ShuffleList(tracks, AlbumViewModel.PlaybackOriginString);
                return;
            }

            var firstTrack = tracks.FirstOrDefault(t => t.Id == AlbumViewModel.Album.LatestTrackId);
            if (firstTrack == null)
                return;
                
            await _mediaPlayer.Play(tracks, firstTrack, AlbumViewModel.Album.LatestTrackPosition);
            _analytics.LogEvent(Event.AlbumResumed);
        }
    }
}