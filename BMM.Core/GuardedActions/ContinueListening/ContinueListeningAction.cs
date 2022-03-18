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
        private readonly IPlaylistClient _playlistClient;
        private readonly IAlbumClient _albumClient;
        private readonly ISettingsStorage _settingsStorage;
        private readonly IExceptionHandler _exceptionHandler;

        public ContinueListeningAction(
            IMediaPlayer mediaPlayer,
            IPlaylistClient playlistClient,
            IAlbumClient albumClient,
            ISettingsStorage settingsStorage,
            IExceptionHandler exceptionHandler)
        {
            _mediaPlayer = mediaPlayer;
            _playlistClient = playlistClient;
            _albumClient = albumClient;
            _settingsStorage = settingsStorage;
            _exceptionHandler = exceptionHandler;
        }
        
        protected override async Task Execute(ContinueListeningTile parameter)
        {
            await _mediaPlayer.Play(
                ((IMediaTrack)parameter.Track).EncloseInArray(),
                parameter.Track,
                PlaybackOrigins.Tile,
                parameter.LastPositionInMs);

            bool autoplayEnabled = await _settingsStorage.GetAutoplayEnabled();

            if (parameter.ShufflePodcastId.HasValue && autoplayEnabled)
                _exceptionHandler.FireAndForgetWithoutUserMessages(EnqueueSongsOfFirstPlaylist);
            else
                _exceptionHandler.FireAndForgetWithoutUserMessages(() => EnqueueRestOfAlbumItems(parameter));
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

        private async Task EnqueueSongsOfFirstPlaylist()
        {
            var tracksToBePlayed = await GetSongsOfFirstPlaylist();
            ShuffleableQueue.ShuffleList(tracksToBePlayed, new Random());

            foreach (var track in tracksToBePlayed)
                await _mediaPlayer.AddToEndOfQueue(track, PlaybackOrigins.Tile);
        }
        
        private async Task<IList<Track>> GetSongsOfFirstPlaylist()
        {
            var playlists = await _playlistClient.GetAll(CachePolicy.UseCacheAndRefreshOutdated);

            if (!playlists.Any())
                return new List<Track>();

            var firstPlaylist = playlists.First();
            var tracks = await _playlistClient.GetTracks(firstPlaylist.Id, CachePolicy.UseCache);

            return tracks;
        }
    }
}