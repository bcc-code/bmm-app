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
    public class ShuffleContributorAction
        : GuardedActionWithParameter<IShuffleContributorActionParameter>,
          IShuffleContributorAction
    {
        private readonly IContributorClient _contributorClient;
        private readonly IMediaPlayer _mediaPlayer;

        public ShuffleContributorAction(
            IContributorClient contributorClient,
            IMediaPlayer mediaPlayer)
        {
            _contributorClient = contributorClient;
            _mediaPlayer = mediaPlayer;
        }
        
        protected override async Task Execute(IShuffleContributorActionParameter parameter)
        {
            var randomTracks = await _contributorClient.GetRandomTracks(parameter.ContributorId);
            var tracksToPlay = randomTracks
                .OfType<IMediaTrack>()
                .ToList();
            await _mediaPlayer.Play(tracksToPlay, tracksToPlay.First(), parameter.PlaybackOrigin);
        }
    }
}