using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BMM.Api.Abstraction;
using BMM.Api.Implementation.Clients.Contracts;
using BMM.Api.Implementation.Models;
using BMM.Core.Constants;
using BMM.Core.Extensions;
using BMM.Core.GuardedActions.Base;
using BMM.Core.GuardedActions.ContinueListening.Interfaces;
using BMM.Core.Implementations.Connection;
using BMM.Core.Implementations.Exceptions;
using BMM.Core.NewMediaPlayer;
using BMM.Core.NewMediaPlayer.Abstractions;

namespace BMM.Core.GuardedActions.ContinueListening
{
    public class ContinueListeningAction
        : GuardedActionWithParameter<ContinueListeningTile>,
          IContinuePlayingAction
    {
        private readonly IMediaPlayer _mediaPlayer;
        private readonly IAlbumClient _albumClient;
        private readonly ISettingsStorage _settingsStorage;
        private readonly IExceptionHandler _exceptionHandler;
        private readonly IEnqueueMusicAction _enqueueMusicAction;

        public ContinueListeningAction(
            IMediaPlayer mediaPlayer,
            IAlbumClient albumClient,
            ISettingsStorage settingsStorage,
            IExceptionHandler exceptionHandler,
            IEnqueueMusicAction enqueueMusicAction)
        {
            _mediaPlayer = mediaPlayer;
            _albumClient = albumClient;
            _settingsStorage = settingsStorage;
            _exceptionHandler = exceptionHandler;
            _enqueueMusicAction = enqueueMusicAction;
        }

        protected override async Task Execute(ContinueListeningTile parameter)
        {
            if (_mediaPlayer.CurrentTrack?.Id == parameter.Track.Id)
            {
                _mediaPlayer.PlayPause();
                return;
            }
            
            await _mediaPlayer.Play(
                ((IMediaTrack)parameter.Track).EncloseInArray(),
                parameter.Track,
                PlaybackOrigins.Tile,
                parameter.LastPositionInMs);

            bool autoplayEnabled = await _settingsStorage.GetAutoplayEnabled();

            if (parameter.ShufflePodcastId != PodcastsConstants.FraKårePodcastId
                && parameter.ShufflePodcastId != PodcastsConstants.ForbildePodcastId
                && parameter.ShufflePodcastId != PodcastsConstants.BergprekenPodcastId
                && parameter.ShufflePodcastId != PodcastsConstants.RomanPodcastId)
            {
                _exceptionHandler.FireAndForgetWithoutUserMessages(() => EnqueueRestOfAlbumItems(parameter));
            }
            else if (autoplayEnabled)
                await _enqueueMusicAction.ExecuteGuarded();
        }

        private async Task EnqueueRestOfAlbumItems(ContinueListeningTile continueListeningTile)
        {
            var album = await _albumClient.GetById(continueListeningTile.Track.ParentId);
            var currentItem = album.Children.FirstOrDefault(x => x.Id == continueListeningTile.Track.Id);
            
            if  (currentItem == null)
                return;

            int indexOfCurrentItem = album.Children.IndexOf(currentItem);

            var itemsToAdd = album
                .Children
                .Select((document, i) => new { Index = i, Document = document })
                .Where(x => x.Index > indexOfCurrentItem)
                .Select(x => x.Document)
                .OfType<IMediaTrack>()
                .ToList();
            
            foreach (var track in itemsToAdd)
                await _mediaPlayer.AddToEndOfQueue(track, PlaybackOrigins.Tile);
        }
    }
}