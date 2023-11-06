using BMM.Api.Abstraction;
using BMM.Api.Implementation.Models;
using BMM.Core.Constants;
using BMM.Core.Extensions;
using BMM.Core.GuardedActions.HighlightedText.Interfaces;
using BMM.Core.Helpers;
using BMM.Core.Implementations.Storage;
using BMM.Core.Messages.MediaPlayer;
using BMM.Core.Models.POs.Base.Interfaces;
using BMM.Core.Models.POs.Tracks;
using BMM.Core.Models.POs.Tracks.Interfaces;
using BMM.Core.NewMediaPlayer.Abstractions;
using BMM.Core.Translation;
using BMM.Core.ViewModels.Base;
using MvvmCross.Commands;
using MvvmCross.Plugin.Messenger;
using MvvmCross.ViewModels;

namespace BMM.Core.ViewModels;

public class HighlightedTextTrackViewModel : DocumentsViewModel, IMvxViewModel<HighlightedTextTrackPO>
{
    private readonly IShowHighlightedTextInfoDialogAction _showHighlightedTextInfoDialogAction;
    private readonly IMediaPlayer _mediaPlayer;
    private string _playButtonTitle;
    private HighlightedTextTrackPO _highlightedTextTrackPO;
    private MvxSubscriptionToken _updateStateToken;
    private bool _isCurrentlyPlaying;
    
    public HighlightedTextTrackViewModel(
        IShowHighlightedTextInfoDialogAction showHighlightedTextInfoDialogAction,
        IMediaPlayer mediaPlayer)
    {
        _showHighlightedTextInfoDialogAction = showHighlightedTextInfoDialogAction;
        _showHighlightedTextInfoDialogAction.AttachDataContext(this);
        
        _mediaPlayer = mediaPlayer;
        PlayPauseCommand = new ExceptionHandlingCommand(() =>
        {
            if (mediaPlayer.IsPlaying && mediaPlayer.CurrentTrack.Id == TrackPO.Id)
            {
                mediaPlayer.Pause();
                return Task.CompletedTask;
            }

            return DocumentAction(TrackPO);
        });

        AddToCommand = new ExceptionHandlingCommand(async () =>
        {
            await CloseCommand.ExecuteAsync();
            await Task.Delay(ViewConstants.DefaultAnimationDurationInMilliseconds);

            await NavigationService.Navigate<TrackCollectionsAddToViewModel, TrackCollectionsAddToViewModel.Parameter>(
                new TrackCollectionsAddToViewModel.Parameter
                {
                    DocumentId = TrackPO.Id,
                    DocumentType = TrackPO.DocumentType,
                    OriginViewModel = PlaybackOriginString()
                });
        });
    }
    
    public ITrackPO TrackPO => _highlightedTextTrackPO.TrackPO;

    public string PlayButtonTitle
    {
        get => _playButtonTitle;
        set => SetProperty(ref _playButtonTitle, value);
    }
    
    public bool IsCurrentlyPlaying
    {
        get => _isCurrentlyPlaying;
        set => SetProperty(ref _isCurrentlyPlaying, value);
    }
    
    public IMvxAsyncCommand PlayPauseCommand { get; }
    public IMvxAsyncCommand AddToCommand { get; }
    public bool IsSong { get; private set; }

    protected override async Task Initialization()
    {
        await base.Initialization();
        if (!AppSettings.HighlightedTextPopupAlreadyShown)
        {
            await _showHighlightedTextInfoDialogAction.ExecuteGuarded();
            AppSettings.HighlightedTextPopupAlreadyShown = true;
        }
    }

    public void Prepare(HighlightedTextTrackPO parameter)
    {
        _highlightedTextTrackPO = parameter;
        IsSong = parameter
            .TrackPO
            .Track
            .Subtype
            .IsOneOf(TrackSubType.Song, TrackSubType.Singsong);
    }

    protected override void AttachEvents()
    {
        base.AttachEvents();
        _updateStateToken = Messenger.Subscribe<PlaybackStatusChangedMessage>(_ => RefreshTrackWithId(null));
    }

    protected override void DetachEvents()
    {
        base.DetachEvents();
        Messenger.UnsubscribeSafe<PlaybackStatusChangedMessage>(_updateStateToken);
    }

    protected override void RefreshTrackWithId(int? currentTrackId)
    {
        base.RefreshTrackWithId(currentTrackId);
        TrackPO.RefreshState();
        SetPlayButtonTitle();
    }

    private void SetPlayButtonTitle()
    {
        IsCurrentlyPlaying = TrackPO.TrackState.IsCurrentlySelected && _mediaPlayer.IsPlaying;
        
        if (TrackPO.TrackState.IsCurrentlySelected)
        {
            PlayButtonTitle = _mediaPlayer.IsPlaying
                ? TextSource[Translations.HighlightedTextTrackViewModel_Pause]
                : TextSource[Translations.TrackCollectionViewModel_Resume];
            return;
        }

        PlayButtonTitle = TextSource[Translations.DocumentsViewModel_Play];
    }

    public override Task<IEnumerable<IDocumentPO>> LoadItems(CachePolicy policy = CachePolicy.UseCacheAndRefreshOutdated)
    {
        RefreshTrackWithId(null);
        var rangeOfItems = new List<IDocumentPO>
        {
            new HighlightedTextHeaderPO(_showHighlightedTextInfoDialogAction, IsSong)
        };

        rangeOfItems.AddRange(_highlightedTextTrackPO.HighlightedTexts);
        return Task.FromResult(rangeOfItems.AsEnumerable());
    }
}