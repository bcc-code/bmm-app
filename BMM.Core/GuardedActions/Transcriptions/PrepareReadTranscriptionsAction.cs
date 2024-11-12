using System.Globalization;
using System.Text;
using BMM.Api.Abstraction;
using BMM.Api.Implementation.Clients.Contracts;
using BMM.Api.Implementation.Models;
using BMM.Core.Extensions;
using BMM.Core.GuardedActions.Base;
using BMM.Core.GuardedActions.Player.Interfaces;
using BMM.Core.GuardedActions.Transcriptions.Interfaces;
using BMM.Core.Helpers;
using BMM.Core.Implementations.Localization.Interfaces;
using BMM.Core.Models.Player.Lyrics;
using BMM.Core.Models.POs.Base.Interfaces;
using BMM.Core.Models.POs.Transcriptions;
using BMM.Core.NewMediaPlayer.Abstractions;
using BMM.Core.Translation;
using BMM.Core.ValueConverters;
using BMM.Core.ViewModels;

namespace BMM.Core.GuardedActions.Transcriptions;

public class PrepareReadTranscriptionsAction
    : GuardedActionWithParameter<ITrackModel>,
      IPrepareReadTranscriptionsAction
{
    private const string Dot = "•";
    private const string PublishedAtFormat = "dd MMMM yyyy";
    private readonly ITracksClient _tracksClient;
    private readonly IBMMLanguageBinder _languageBinder;
    private readonly IMediaPlayer _mediaPlayer;
    private readonly IGetLyricsLinkAction _getLyricsLinkAction;

    public PrepareReadTranscriptionsAction(
        ITracksClient tracksClient,
        IBMMLanguageBinder languageBinder,
        IMediaPlayer mediaPlayer,
        IGetLyricsLinkAction getLyricsLinkAction)
    {
        _tracksClient = tracksClient;
        _languageBinder = languageBinder;
        _mediaPlayer = mediaPlayer;
        _getLyricsLinkAction = getLyricsLinkAction;
    }
    
    private ReadTranscriptionViewModel DataContext => this.GetDataContext();
    
    protected override async Task Execute(ITrackModel track)
    {
        var response = await _tracksClient.GetTranscriptions(track.Id);

        var titleConverter = new TrackToTitleValueConverter();
        string trackTitle = (string)titleConverter.Convert(track,
            typeof(string),
            DataContext,
            CultureInfo.CurrentUICulture);

        var items = new List<IBasePO>
        {
            new TranscriptionHeaderPO(trackTitle, PrepareSubtitle(track))
        };

        DataContext.LyricsLink = await _getLyricsLinkAction.ExecuteGuarded(track);
        DataContext.HasTimeframes = response.Last().End != 0;
        DataContext.IsAutoTranscribed = track.Subtype.IsNoneOf(TrackSubType.Singsong, TrackSubType.Song);
        DataContext.HeaderText = GetHeaderText();
        items.AddRange(response.Select(t => new ReadTranscriptionsPO(t, TranscriptionClickedAction)).ToList());
        DataContext.Transcriptions.ReplaceWith(items);
    }

    private string GetHeaderText()
    {
        if (DataContext.IsAutoTranscribed)
            return _languageBinder[Translations.HighlightedTextTrackViewModel_AutoTranscribed];

        return DataContext.LyricsLink.LyricsLinkType switch
        {
            LyricsLinkType.SongTreasures => _languageBinder[Translations.ReadTranscriptionViewModel_SongTreasures],
            LyricsLinkType.Generic => _languageBinder[Translations.ReadTranscriptionViewModel_Lyrics],
            _ => default
        };
    }

    private void TranscriptionClickedAction(Transcription transcription)
    {
        if (DataContext.HasTimeframes)
            _mediaPlayer.SeekTo((long)TimeSpan.FromSeconds(transcription.Start).TotalMilliseconds);
    }

    private string PrepareSubtitle(ITrackModel trackModel)
    {
        var subtitleStringBuilder = new StringBuilder();

        subtitleStringBuilder.Append($"{_languageBinder[Translations.TrackInfoViewModel_Lyricist]}: {trackModel.Meta.Lyricist}");
        subtitleStringBuilder.Append($" {Dot} ");
        subtitleStringBuilder.Append($"{_languageBinder[Translations.TrackInfoViewModel_Composer]}: {trackModel.Meta.Composer}");
        subtitleStringBuilder.Append($" {Dot} ");
        subtitleStringBuilder.Append($"{trackModel.PublishedAt.ToNorwegianTime().ToString(PublishedAtFormat)}");
        
        return subtitleStringBuilder.ToString();
    }
}