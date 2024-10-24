using BMM.Api.Implementation.Models;
using BMM.Core.Extensions;
using BMM.Core.GuardedActions.ContinueListening.Interfaces;
using BMM.Core.GuardedActions.Tracks.Interfaces;
using BMM.Core.Implementations.FileStorage;
using BMM.Core.Models.Enums;
using BMM.Core.Models.POs.Base;
using BMM.Core.Models.POs.Tiles.Interfaces;
using BMM.Core.NewMediaPlayer.Abstractions;
using MvvmCross.Commands;

namespace BMM.Core.Models.POs.Tiles
{
    public class ContinueListeningTilePO : DocumentPO, ITilePO<ContinueListeningTile>, ITrackHolderPO
    {
        public const string NotificationBadgeIcon = "⬤";
        public const string PlayingIcon = "▶";
        
        private readonly IMediaPlayer _mediaPlayer;
        private readonly IStorageManager _storageManager;
        private bool _isCurrentlyPlaying;
        private bool _isDownloaded;
        private TileStatusTextIcon _tileStatusTextIcon;

        public ContinueListeningTilePO(
            IMvxAsyncCommand<Document> optionsClickedCommand,
            ITileClickedAction tileClickedAction,
            IContinuePlayingAction continuePlayingAction,
            IShuffleButtonClickedAction shuffleButtonClickedAction,
            IShowTrackInfoAction showTrackInfoAction,
            IMediaPlayer mediaPlayer,
            IStorageManager storageManager,
            ContinueListeningTile continueListeningTile) : base(continueListeningTile)
        {
            _mediaPlayer = mediaPlayer;
            _storageManager = storageManager;
            TileClickedCommand = new MvxAsyncCommand(async () =>
            {
                await tileClickedAction.ExecuteGuarded(Tile);
            });
            
            ContinueListeningCommand = new MvxAsyncCommand(async () =>
            {
                await continuePlayingAction.ExecuteGuarded(Tile);
            });

            ShuffleButtonClickedCommand = new MvxAsyncCommand(async () =>
            {
                await shuffleButtonClickedAction.ExecuteGuarded(Tile);
            });
            
            ShowTrackInfoCommand = new MvxAsyncCommand(async () =>
            {
                await showTrackInfoAction.ExecuteGuarded(Tile!.Track);
            });

            OptionButtonClickedCommand = new MvxAsyncCommand(async () =>
            {
                await optionsClickedCommand.ExecuteAsync(Tile);
            });
            
            Tile = continueListeningTile;
            RefreshState();
        }

        public IMvxAsyncCommand TileClickedCommand { get; }
        public IMvxAsyncCommand ContinueListeningCommand { get; }
        public IMvxAsyncCommand ShuffleButtonClickedCommand { get; }
        public IMvxAsyncCommand ShowTrackInfoCommand { get; }
        public IMvxAsyncCommand OptionButtonClickedCommand { get; }
        public ContinueListeningTile Tile { get; }

        public bool IsBibleStudyProjectTile => Tile.Track.IsBibleStudyProjectTrack();
        public bool ShuffleButtonVisible => !IsBibleStudyProjectTile && Tile.ShufflePodcastId.HasValue;
        public bool DownloadedIconVisible => !IsBibleStudyProjectTile && IsDownloaded;
        public bool ReferenceButtonVisible => !IsBibleStudyProjectTile && Tile.Track.HasExternalRelations();
        public bool ShouldShowSubtitle => Tile.LastPositionInMs != default;

        public TileStatusTextIcon TileStatusTextIcon
        {
            get => _tileStatusTextIcon;
            set => SetProperty(ref _tileStatusTextIcon, value);
        }

        public bool IsCurrentlyPlaying
        {
            get => _isCurrentlyPlaying;
            set => SetProperty(ref _isCurrentlyPlaying, value);
        }
        
        public bool IsDownloaded
        {
            get => _isDownloaded;
            set => SetProperty(ref _isDownloaded, value);
        }
        
        public Task RefreshState()
        {
            bool isCurrentlySelected = _mediaPlayer.CurrentTrack != null && _mediaPlayer.CurrentTrack.Id.Equals(Tile.Track.Id);
            TileStatusTextIcon = GetTileStatusIcon(isCurrentlySelected);
            IsCurrentlyPlaying = isCurrentlySelected && _mediaPlayer.IsPlaying;
            IsDownloaded = _storageManager.SelectedStorage.IsDownloaded(Tile.Track);
            return Task.CompletedTask;
        }

        private TileStatusTextIcon GetTileStatusIcon(bool isCurrentlySelected)
        {
            if (isCurrentlySelected)
                return TileStatusTextIcon.Play;
            else if (!Tile.Track.HasListened)
                return TileStatusTextIcon.Dot;

            return TileStatusTextIcon.None;
        }
    }
}