using BMM.Api.Implementation.Models;
using BMM.Core.GuardedActions.ContinueListening.Interfaces;
using BMM.Core.GuardedActions.Tracks.Interfaces;
using BMM.Core.Implementations.FileStorage;
using BMM.Core.Models.POs.ContinueListening;
using BMM.Core.NewMediaPlayer.Abstractions;
using MvvmCross.Commands;

namespace BMM.Core.Implementations.Factories.ContinueListening
{
    public class ContinueListeningTilePOFactory : IContinueListeningTilePOFactory
    {
        private readonly ITileClickedAction _tileClickedAction;
        private readonly IContinuePlayingAction _continuePlayingAction;
        private readonly IShuffleButtonClickedAction _shuffleButtonClickedAction;
        private readonly IShowTrackInfoAction _showTrackInfoAction;
        private readonly IMediaPlayer _mediaPlayer;
        private readonly IStorageManager _storageManager;

        public ContinueListeningTilePOFactory(
            ITileClickedAction tileClickedAction,
            IContinuePlayingAction continuePlayingAction,
            IShuffleButtonClickedAction shuffleButtonClickedAction,
            IShowTrackInfoAction showTrackInfoAction,
            IMediaPlayer mediaPlayer,
            IStorageManager storageManager)
        {
            _tileClickedAction = tileClickedAction;
            _continuePlayingAction = continuePlayingAction;
            _shuffleButtonClickedAction = shuffleButtonClickedAction;
            _showTrackInfoAction = showTrackInfoAction;
            _mediaPlayer = mediaPlayer;
            _storageManager = storageManager;
        }
        
        public ContinueListeningTilePO Create(IMvxAsyncCommand<Document> optionsClickedCommand, ContinueListeningTile continueListeningTile)
        {
            return new ContinueListeningTilePO(
                optionsClickedCommand,
                _tileClickedAction,
                _continuePlayingAction,
                _shuffleButtonClickedAction,
                _showTrackInfoAction,
                _mediaPlayer,
                _storageManager,
                continueListeningTile);
        }
    }
}