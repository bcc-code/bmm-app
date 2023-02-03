using System.Threading.Tasks;
using BMM.Api;
using BMM.Api.Abstraction;
using BMM.Api.Implementation.Clients.Contracts;
using BMM.Api.Implementation.Models;
using BMM.Core.Extensions;
using BMM.Core.GuardedActions.Base;
using BMM.Core.GuardedActions.ContinueListening.Interfaces;
using BMM.Core.Implementations.Exceptions;
using BMM.Core.NewMediaPlayer.Abstractions;

namespace BMM.Core.GuardedActions.ContinueListening
{
    public class AddToQueueAdditionalMusicAction : GuardedActionWithParameter<(int AlreadyAddedTracksCount, string PlaybackOrigin)>, IAddToQueueAdditionalMusic
    {
        private readonly IMediaPlayer _mediaPlayer;
        private readonly ITracksClient _tracksClient;
        private readonly IExceptionHandler _exceptionHandler;

        public AddToQueueAdditionalMusicAction(
            IMediaPlayer mediaPlayer,
            ITracksClient tracksClient,
            IExceptionHandler exceptionHandler)
        {
            _mediaPlayer = mediaPlayer;
            _tracksClient = tracksClient;
            _exceptionHandler = exceptionHandler;
        }
        
        protected override Task Execute((int AlreadyAddedTracksCount, string PlaybackOrigin) parameters)
        {
            _exceptionHandler.FireAndForgetWithoutUserMessages(() => LoadAdditionalTracks(parameters));
            return Task.CompletedTask;
        }

        private async Task LoadAdditionalTracks((int AlreadyAddedTracksCount, string PlaybackOrigin) parameters)
        {
            var tracksToAdd = await _tracksClient.GetAll(
                CachePolicy.IgnoreCache,
                ApiConstants.LoadMoreSize - parameters.AlreadyAddedTracksCount,
                parameters.AlreadyAddedTracksCount,
                TrackSubType.Song.EncloseInArray());

            foreach (var track in tracksToAdd)
                await _mediaPlayer.AddToEndOfQueue(track, parameters.PlaybackOrigin);
        }
    }
}