using System.ComponentModel;
using BMM.Api.Abstraction;
using BMM.Api.Implementation.Models;
using BMM.Core.Extensions;
using BMM.Core.GuardedActions.Transcriptions.Interfaces;
using BMM.Core.Helpers;
using BMM.Core.Helpers.Interfaces;
using BMM.Core.Implementations.Analytics;
using BMM.Core.Implementations.TrackInformation.Strategies;
using BMM.Core.Implementations.UI;
using BMM.Core.Models.Player.Lyrics;
using BMM.Core.Models.POs.Base.Interfaces;
using BMM.Core.Models.POs.Transcriptions;
using BMM.Core.NewMediaPlayer.Abstractions;
using BMM.Core.ViewModels.Parameters;
using MvvmCross.Commands;
using MvvmCross.ViewModels;

namespace BMM.Core.ViewModels;

public class ReadTranscriptionViewModel : PlayerBaseViewModel, IMvxViewModel<TranscriptionParameter>
{
    private readonly IMediaPlayer _mediaPlayer;
    private readonly IPrepareReadTranscriptionsAction _prepareReadTranscriptionsAction;
    private readonly IAdjustHighlightedTranscriptionsAction _adjustHighlightedTranscriptionsAction;
    private readonly IAnalytics _analytics;
    private MiniPlayerTrackInfoProvider _miniPlayerTrackInfoProvider;
    private bool _initialized;
    private LyricsLink _lyricsLink;
    private string _headerText;
    private bool _isAutoTranscribed;

    public ReadTranscriptionViewModel(IMediaPlayer mediaPlayer,
        IPrepareReadTranscriptionsAction prepareReadTranscriptionsAction,
        IAdjustHighlightedTranscriptionsAction adjustHighlightedTranscriptionsAction,
        IAnalytics analytics,
        IUriOpener uriOpener) : base(mediaPlayer)
    {
        _mediaPlayer = mediaPlayer;
        _prepareReadTranscriptionsAction = prepareReadTranscriptionsAction;
        _adjustHighlightedTranscriptionsAction = adjustHighlightedTranscriptionsAction;
        _analytics = analytics;
        _prepareReadTranscriptionsAction.AttachDataContext(this);
        _adjustHighlightedTranscriptionsAction.AttachDataContext(this);
        HeaderClickedCommand = new ExceptionHandlingCommand(() =>
        {
            if (IsAutoTranscribed)
                return Task.CompletedTask;
            
            uriOpener.OpenUri(new Uri(LyricsLink.Link));
            return Task.CompletedTask;
        });
    }

    public override ITrackInfoProvider TrackInfoProvider => _miniPlayerTrackInfoProvider ??= new MiniPlayerTrackInfoProvider();
    public MvxInteraction<int> AdjustScrollPositionInteraction { get; } = new();
    public IBmmObservableCollection<IBasePO> Transcriptions { get; } = new BmmObservableCollection<IBasePO>();
    public bool HasTimeframes { get; set; }
    public ITrackModel Track { get; private set; }

    public bool IsAutoTranscribed
    {
        get => _isAutoTranscribed;
        set => SetProperty(ref _isAutoTranscribed, value);
    }

    public string HeaderText
    {
        get => _headerText;
        set => SetProperty(ref _headerText, value);
    }

    public LyricsLink LyricsLink
    {
        get => _lyricsLink;
        set => SetProperty(ref _lyricsLink, value);
    }

    public IMvxAsyncCommand HeaderClickedCommand { get; }

    public void Prepare(TranscriptionParameter parameter)
    {
        Track = parameter.Track;
        _analytics.LogEvent("open ReadTranscriptionViewModel",
            new Dictionary<string, object> {{"trackId", parameter.Track.Id}, {"trackType", parameter.Track.Subtype}});
    }

    public override async Task Initialize()
    {
        await base.Initialize();
        await _prepareReadTranscriptionsAction.ExecuteGuarded(Track);
        await _adjustHighlightedTranscriptionsAction.ExecuteGuarded(CurrentPosition);
        _initialized = true;
    }

    protected override async void AttachEvents()
    {
        base.AttachEvents();
        PropertyChanged -= OnPropertyChanged;
        PropertyChanged += OnPropertyChanged;
        
        if (_initialized)
            await _adjustHighlightedTranscriptionsAction.ExecuteGuarded(CurrentPosition);
    }

    protected override void DetachEvents()
    {
        base.DetachEvents();
        PropertyChanged -= OnPropertyChanged;
    }
    
    private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(CurrentPosition)
            && _mediaPlayer.CurrentTrack?.Id == Track.Id
            && _initialized)
        {
            _adjustHighlightedTranscriptionsAction.ExecuteGuarded(CurrentPosition);
        }
    }
}