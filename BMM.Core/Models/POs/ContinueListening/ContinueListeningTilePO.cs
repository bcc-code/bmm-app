using BMM.Api.Implementation.Models;
using BMM.Core.GuardedActions.ContinueListening.Interfaces;
using BMM.Core.GuardedActions.Tracks.Interfaces;
using BMM.Core.Models.POs.Base;
using BMM.Core.NewMediaPlayer.Abstractions;
using MvvmCross.Commands;

namespace BMM.Core.Models.POs.ContinueListening
{
    public class ContinueListeningTilePO : DocumentPO, ITrackHolderPO
    {
        private readonly IMediaPlayer _mediaPlayer;
        private bool _isCurrentlySelected;

        public ContinueListeningTilePO(
            IMvxAsyncCommand<Document> optionsClickedCommand,
            ITileClickedAction tileClickedAction,
            IContinuePlayingAction continuePlayingAction,
            IShuffleButtonClickedAction shuffleButtonClickedAction,
            IShowTrackInfoAction showTrackInfoAction,
            IMediaPlayer mediaPlayer,
            ContinueListeningTile continueListeningTile) : base(continueListeningTile)
        {
            _mediaPlayer = mediaPlayer;
            TileClickedCommand = new MvxAsyncCommand(async () =>
            {
                await tileClickedAction.ExecuteGuarded(ContinueListeningTile);
            });
            
            ContinueListeningCommand = new MvxAsyncCommand(async () =>
            {
                await continuePlayingAction.ExecuteGuarded(ContinueListeningTile);
            });

            ShuffleButtonClickedCommand = new MvxAsyncCommand(async () =>
            {
                await shuffleButtonClickedAction.ExecuteGuarded(ContinueListeningTile);
            });
            
            ShowTrackInfoCommand = new MvxAsyncCommand(async () =>
            {
                await showTrackInfoAction.ExecuteGuarded(ContinueListeningTile!.Track);
            });

            OptionButtonClickedCommand = new MvxAsyncCommand(async () =>
            {
                await optionsClickedCommand.ExecuteAsync(ContinueListeningTile);
            });
            
            ContinueListeningTile = continueListeningTile;
            RefreshState();
        }
        
        public IMvxAsyncCommand TileClickedCommand { get; }
        public IMvxAsyncCommand ContinueListeningCommand { get; }
        public IMvxAsyncCommand ShuffleButtonClickedCommand { get; }
        public IMvxAsyncCommand ShowTrackInfoCommand { get; }
        public IMvxAsyncCommand OptionButtonClickedCommand { get; }
        public ContinueListeningTile ContinueListeningTile { get; }

        public bool IsCurrentlySelected
        {
            get => _isCurrentlySelected;
            set => SetProperty(ref _isCurrentlySelected, value);
        }
        
        public void RefreshState()
        {
            IsCurrentlySelected = _mediaPlayer.CurrentTrack != null && _mediaPlayer.CurrentTrack.Id.Equals(ContinueListeningTile.Track.Id);
        }
    }
}