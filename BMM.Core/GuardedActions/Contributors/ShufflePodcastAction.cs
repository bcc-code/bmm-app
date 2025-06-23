using System.Linq;
using System.Threading.Tasks;
using BMM.Api.Abstraction;
using BMM.Api.Implementation.Clients.Contracts;
using BMM.Core.GuardedActions.Base;
using BMM.Core.GuardedActions.Contributors.Interfaces;
using BMM.Core.Models.Contributors.Interfaces;
using BMM.Core.NewMediaPlayer.Abstractions;

namespace BMM.Core.GuardedActions.Contributors
{
    public class ShufflePodcastAction
        : GuardedActionWithParameterAndResult<IShuffleActionParameter, IMediaTrack>,
          IShufflePodcastAction
    {
        private readonly IPodcastClient _podcastClient;
        private readonly IMediaPlayer _mediaPlayer;

        public ShufflePodcastAction(
            IMediaPlayer mediaPlayer,
            IPodcastClient podcastClient)
        {
            _mediaPlayer = mediaPlayer;
            _podcastClient = podcastClient;
        }
        
        protected override async Task<IMediaTrack> Execute(IShuffleActionParameter parameter)
        {
            var podcastTracks = await _podcastClient.GetShuffle(parameter.Id);
            var tracksToPlay = podcastTracks
                .OfType<IMediaTrack>()
                .ToList();
            var trackToPlay = tracksToPlay.First();
            await _mediaPlayer.Play(tracksToPlay, trackToPlay, parameter.PlaybackOrigin);
            return trackToPlay;
        }
    }
}