using BMM.Api.Abstraction;
using BMM.Api.Implementation.Models;
using BMM.Core.Constants;
using BMM.Core.Extensions;
using BMM.Core.GuardedActions.Base;
using BMM.Core.GuardedActions.ContinueListening.Interfaces;
using BMM.Core.NewMediaPlayer.Abstractions;

namespace BMM.Core.GuardedActions.ContinueListening
{
    public class ContinueListeningAction
        : GuardedActionWithParameter<ContinueListeningTile>,
          IContinuePlayingAction
    {
        private readonly IMediaPlayer _mediaPlayer;
        private readonly IHandleAutoplayAction _handleAutoplayAction;

        public ContinueListeningAction(
            IMediaPlayer mediaPlayer,
            IHandleAutoplayAction handleAutoplayAction)
        {
            _mediaPlayer = mediaPlayer;
            _handleAutoplayAction = handleAutoplayAction;
        }

        protected override async Task Execute(ContinueListeningTile continueListeningTile)
        {
            if (_mediaPlayer.CurrentTrack?.Id == continueListeningTile.Track.Id)
            {
                _mediaPlayer.PlayPause();
                return;
            }
            
            await _mediaPlayer.Play(
                ((IMediaTrack)continueListeningTile.Track).EncloseInArray(),
                continueListeningTile.Track,
                PlaybackOrigins.Tile,
                continueListeningTile.LastPositionInMs);
            
            await _handleAutoplayAction.ExecuteGuarded(continueListeningTile);
        }
    }
}