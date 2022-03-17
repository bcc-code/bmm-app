using System.Linq;
using System.Threading.Tasks;
using BMM.Api.Abstraction;
using BMM.Api.Implementation.Clients.Contracts;
using BMM.Api.Implementation.Models;
using BMM.Core.Constants;
using BMM.Core.GuardedActions.Base;
using BMM.Core.GuardedActions.ContinueListening.Interfaces;
using BMM.Core.NewMediaPlayer.Abstractions;

namespace BMM.Core.GuardedActions.ContinueListening
{
    public class ShuffleButtonClickedAction
        : GuardedActionWithParameter<ContinueListeningTile>,
          IShuffleButtonClickedAction
    {
        private readonly IPodcastClient _podcastClient;
        private readonly IMediaPlayer _mediaPlayer;

        public ShuffleButtonClickedAction(
            IPodcastClient podcastClient,
            IMediaPlayer mediaPlayer)
        {
            _podcastClient = podcastClient;
            _mediaPlayer = mediaPlayer;
        }
        
        protected override async Task Execute(ContinueListeningTile parameter)
        {
            if (!parameter.ShufflePodcastId.HasValue)
                return;
                
            var tracks = await _podcastClient.GetShuffle(parameter.ShufflePodcastId.Value);
            await _mediaPlayer.Play(
                tracks.OfType<IMediaTrack>().ToList(),
                tracks.First(),
                PlaybackOrigins.Tile);
        }
    }
}