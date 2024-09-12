using System.ComponentModel;
using BMM.Core.Extensions;
using BMM.Core.GuardedActions.Transcriptions.Interfaces;
using BMM.Core.Helpers;
using BMM.Core.Helpers.Interfaces;
using BMM.Core.Implementations.Analytics;
using BMM.Core.Implementations.TrackInformation.Strategies;
using BMM.Core.Models.POs.Transcriptions;
using BMM.Core.NewMediaPlayer.Abstractions;
using BMM.Core.ViewModels.Parameters;
using MvvmCross.ViewModels;

namespace BMM.Core.ViewModels;

public class ReadTranscriptionViewModel : PlayerBaseViewModel, IMvxViewModel<TranscriptionParameter>
{
    private readonly IMediaPlayer _mediaPlayer;
    private readonly IPrepareReadTranscriptionsAction _prepareReadTranscriptionsAction;
    private readonly IAdjustHighlightedTranscriptionsAction _adjustHighlightedTranscriptionsAction;
    private readonly IAnalytics _analytics;
    private int _trackId;
    private MiniPlayerTrackInfoProvider _miniPlayerTrackInfoProvider;
    private bool _initialized;

    public ReadTranscriptionViewModel(IMediaPlayer mediaPlayer,
        IPrepareReadTranscriptionsAction prepareReadTranscriptionsAction,
        IAdjustHighlightedTranscriptionsAction adjustHighlightedTranscriptionsAction,
        IAnalytics analytics) : base(mediaPlayer)
    {
        _mediaPlayer = mediaPlayer;
        _prepareReadTranscriptionsAction = prepareReadTranscriptionsAction;
        _adjustHighlightedTranscriptionsAction = adjustHighlightedTranscriptionsAction;
        _analytics = analytics;
        _prepareReadTranscriptionsAction.AttachDataContext(this);
        _adjustHighlightedTranscriptionsAction.AttachDataContext(this);
    }

    public override ITrackInfoProvider TrackInfoProvider => _miniPlayerTrackInfoProvider ??= new MiniPlayerTrackInfoProvider();
    public MvxInteraction<int> AdjustScrollPositionInteraction { get; } = new();
    public IBmmObservableCollection<ReadTranscriptionsPO> Transcriptions { get; } = new BmmObservableCollection<ReadTranscriptionsPO>();

    public void Prepare(TranscriptionParameter parameter)
    {
        _trackId = parameter.TrackId;
        _analytics.LogEvent("open ReadTranscriptionViewModel",
            new Dictionary<string, object> {{"trackId", parameter.TrackId}, {"trackType", parameter.TrackType}});
    }

    public override async Task Initialize()
    {
        await base.Initialize();
        await _prepareReadTranscriptionsAction.ExecuteGuarded(_trackId);
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
            && _mediaPlayer.CurrentTrack?.Id == _trackId
            && _initialized)
        {
            _adjustHighlightedTranscriptionsAction.ExecuteGuarded(CurrentPosition);
        }
    }
}