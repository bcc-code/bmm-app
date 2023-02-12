using System.Threading.Tasks;
using BMM.Api.Implementation.Clients.Contracts;
using BMM.Core.Constants;
using BMM.Core.GuardedActions.Base;
using BMM.Core.GuardedActions.ContinueListening.Interfaces;
using BMM.Core.Implementations.Exceptions;
using BMM.Core.NewMediaPlayer.Abstractions;

namespace BMM.Core.GuardedActions.ContinueListening
{
    public class EnqueueMusicAction : GuardedAction, IEnqueueMusicAction
    {
        private readonly IMediaPlayer _mediaPlayer;
        private readonly ITracksClient _tracksClient;
        private readonly IExceptionHandler _exceptionHandler;

        public EnqueueMusicAction(
            IMediaPlayer mediaPlayer,
            ITracksClient tracksClient,
            IExceptionHandler exceptionHandler)
        {
            _mediaPlayer = mediaPlayer;
            _tracksClient = tracksClient;
            _exceptionHandler = exceptionHandler;
        }
        
        protected override Task Execute()
        {
            _exceptionHandler.FireAndForgetWithoutUserMessages(EnqueueMusic);
            return Task.CompletedTask;
        }
        
        private async Task EnqueueMusic()
        {
            var tracksToBePlayed = await _tracksClient.GetRecommendations();

            foreach (var track in tracksToBePlayed)
                await _mediaPlayer.AddToEndOfQueue(track, PlaybackOrigins.Tile);
        }
    }
}