using System.Linq;
using System.Threading.Tasks;
using BMM.Api;
using BMM.Api.Abstraction;
using BMM.Api.Implementation.Clients.Contracts;
using BMM.Api.Implementation.Models;
using BMM.Core.Constants;
using BMM.Core.GuardedActions.Base;
using BMM.Core.GuardedActions.ContinueListening.Interfaces;
using BMM.Core.Implementations.Connection;
using BMM.Core.NewMediaPlayer.Abstractions;

namespace BMM.Core.GuardedActions.ContinueListening
{
    public class ShuffleButtonClickedAction
        : GuardedActionWithParameter<ContinueListeningTile>,
          IShuffleButtonClickedAction
    {
        private const int ItemToLoadWhenAutoplayEnabled = 1; 
        private readonly IPodcastClient _podcastClient;
        private readonly IMediaPlayer _mediaPlayer;
        private readonly ISettingsStorage _settingsStorage;
        private readonly IEnqueueMusicAction _enqueueMusicAction;

        public ShuffleButtonClickedAction(
            IPodcastClient podcastClient,
            IMediaPlayer mediaPlayer,
            ISettingsStorage settingsStorage,
            IEnqueueMusicAction enqueueMusicAction)
        {
            _podcastClient = podcastClient;
            _mediaPlayer = mediaPlayer;
            _settingsStorage = settingsStorage;
            _enqueueMusicAction = enqueueMusicAction;
        }
        
        protected override async Task Execute(ContinueListeningTile parameter)
        {
            if (!parameter.ShufflePodcastId.HasValue)
                return;
                
            bool autoplayEnabled = await _settingsStorage.GetAutoplayEnabled();

            int itemsToLoad = autoplayEnabled
                ? ItemToLoadWhenAutoplayEnabled
                : ApiConstants.LoadMoreSize;
            
            var tracks = await _podcastClient.GetShuffle(parameter.ShufflePodcastId.Value, itemsToLoad);
            
            await _mediaPlayer.Play(
                tracks.OfType<IMediaTrack>().ToList(),
                tracks.First(),
                PlaybackOrigins.Tile);

            if (autoplayEnabled)
                await _enqueueMusicAction.ExecuteGuarded();
        }
    }
}