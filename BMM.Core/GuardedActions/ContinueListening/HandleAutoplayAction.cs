using BMM.Api.Abstraction;
using BMM.Api.Implementation.Clients.Contracts;
using BMM.Api.Implementation.Models;
using BMM.Core.Constants;
using BMM.Core.GuardedActions.Base;
using BMM.Core.GuardedActions.ContinueListening.Interfaces;
using BMM.Core.Implementations.Connection;
using BMM.Core.Implementations.Exceptions;
using BMM.Core.NewMediaPlayer.Abstractions;

namespace BMM.Core.GuardedActions.ContinueListening;

public class HandleAutoplayAction 
    : GuardedActionWithParameter<ContinueListeningTile>,
      IHandleAutoplayAction
{
    private readonly IAlbumClient _albumClient;
    private readonly ISettingsStorage _settingsStorage;
    private readonly IExceptionHandler _exceptionHandler;
    private readonly IEnqueueMusicAction _enqueueMusicAction;
    private readonly IMediaPlayer _mediaPlayer;

    public HandleAutoplayAction(
        IAlbumClient albumClient,
        ISettingsStorage settingsStorage,
        IExceptionHandler exceptionHandler,
        IEnqueueMusicAction enqueueMusicAction,
        IMediaPlayer mediaPlayer)
    {
        _albumClient = albumClient;
        _settingsStorage = settingsStorage;
        _exceptionHandler = exceptionHandler;
        _enqueueMusicAction = enqueueMusicAction;
        _mediaPlayer = mediaPlayer;
    }
    
    protected override async Task Execute(ContinueListeningTile continueListeningTile)
    {
        bool autoplayEnabled = await _settingsStorage.GetAutoplayEnabled();
        
        if (!continueListeningTile.ShufflePodcastId.HasValue)
            _exceptionHandler.FireAndForgetWithoutUserMessages(() => EnqueueRestOfAlbumItems(continueListeningTile));
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