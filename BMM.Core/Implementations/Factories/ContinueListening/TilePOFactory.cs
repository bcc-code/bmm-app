using System;
using BMM.Api.Implementation.Models;
using BMM.Core.GuardedActions.ContinueListening.Interfaces;
using BMM.Core.GuardedActions.Tracks.Interfaces;
using BMM.Core.Helpers;
using BMM.Core.Implementations.Badge;
using BMM.Core.Implementations.FileStorage;
using BMM.Core.Implementations.UI;
using BMM.Core.Models.POs.Tiles;
using BMM.Core.Models.POs.Tiles.Interfaces;
using BMM.Core.NewMediaPlayer.Abstractions;
using MvvmCross.Commands;

namespace BMM.Core.Implementations.Factories.ContinueListening
{
    public class TilePOFactory : ITilePOFactory
    {
        private readonly ITileClickedAction _tileClickedAction;
        private readonly IContinuePlayingAction _continuePlayingAction;
        private readonly IShuffleButtonClickedAction _shuffleButtonClickedAction;
        private readonly IShowTrackInfoAction _showTrackInfoAction;
        private readonly IMediaPlayer _mediaPlayer;
        private readonly IStorageManager _storageManager;
        private readonly IUriOpener _uriOpener;
        private readonly IDeepLinkHandler _deepLinkHandler;
        private readonly IBadgeService _badgeService;

        public TilePOFactory(
            ITileClickedAction tileClickedAction,
            IContinuePlayingAction continuePlayingAction,
            IShuffleButtonClickedAction shuffleButtonClickedAction,
            IShowTrackInfoAction showTrackInfoAction,
            IMediaPlayer mediaPlayer,
            IStorageManager storageManager,
            IUriOpener uriOpener,
            IDeepLinkHandler deepLinkHandler,
            IBadgeService badgeService)
        {
            _tileClickedAction = tileClickedAction;
            _continuePlayingAction = continuePlayingAction;
            _shuffleButtonClickedAction = shuffleButtonClickedAction;
            _showTrackInfoAction = showTrackInfoAction;
            _mediaPlayer = mediaPlayer;
            _storageManager = storageManager;
            _uriOpener = uriOpener;
            _deepLinkHandler = deepLinkHandler;
            _badgeService = badgeService;
        }
        
        public ITilePO Create(IMvxAsyncCommand<Document> optionsClickedCommand, Document tile)
        {
            switch (tile)
            {
                case ContinueListeningTile continueListeningTile:
                {
                    return new ContinueListeningTilePO(
                        optionsClickedCommand,
                        _tileClickedAction,
                        _continuePlayingAction,
                        _shuffleButtonClickedAction,
                        _showTrackInfoAction,
                        _mediaPlayer,
                        _storageManager,
                        _badgeService,
                        continueListeningTile);
                }
                case MessageTile messageTile:
                {
                    return new MessageTilePO(messageTile, _deepLinkHandler, _uriOpener);
                }
                case VideoTile videoTile:
                {
                    return new VideoTilePO(videoTile, _deepLinkHandler, _uriOpener);
                }
            }

            return default;
        }
    }
}