using System.Threading.Tasks;
using BMM.Api;
using BMM.Api.Abstraction;
using BMM.Api.Implementation.Clients.Contracts;
using BMM.Api.Implementation.Models;
using BMM.Core.Constants;
using BMM.Core.Extensions;
using BMM.Core.GuardedActions.Base;
using BMM.Core.GuardedActions.ContinueListening.Interfaces;
using BMM.Core.Implementations.Exceptions;
using BMM.Core.NewMediaPlayer.Abstractions;

namespace BMM.Core.GuardedActions.ContinueListening
{
    public class AddToQueueAdditionalMusicAction : GuardedActionWithParameter<string>, IAddToQueueAdditionalMusic
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
        
        protected override Task Execute(string playbackOrigin)
        {
            _exceptionHandler.FireAndForgetWithoutUserMessages(() => LoadAdditionalTracks(playbackOrigin));
            return Task.CompletedTask;
        }

        private async Task LoadAdditionalTracks(string playbackOrigin)
        {
            var tracksToAdd = await _tracksClient.GetAll(
                CachePolicy.IgnoreCache,
                ApiConstants.LoadMoreSize,
                NumericConstants.Zero,
                TrackSubType.Song.EncloseInArray());

            foreach (var track in tracksToAdd)
                await _mediaPlayer.AddToEndOfQueue(track, playbackOrigin, true);
        }
    }
}