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
using BMM.Core.Implementations.Factories.Transcriptions;
using BMM.Core.Implementations.Localization.Interfaces;
using BMM.Core.Models.Player.Lyrics;
using BMM.Core.Models.POs.Base.Interfaces;
using BMM.Core.Models.POs.Transcriptions;
using BMM.Core.NewMediaPlayer.Abstractions;
using BMM.Core.Translation;
using BMM.Core.ValueConverters;
using BMM.Core.ViewModels;
using Microsoft.IdentityModel.Tokens;

namespace BMM.Core.GuardedActions.Transcriptions;

public class PrepareReadTranscriptionsAction
    : GuardedActionWithParameter<ITrackModel>,
      IPrepareReadTranscriptionsAction
{
    private const string Dot = "•";
    private readonly ITracksClient _tracksClient;
    private readonly IBMMLanguageBinder _languageBinder;
    private readonly IMediaPlayer _mediaPlayer;
    private readonly IGetLyricsLinkAction _getLyricsLinkAction;
    private readonly ITranscriptionPOFactory _transcriptionPOFactory;

    public PrepareReadTranscriptionsAction(
        ITracksClient tracksClient,
        IBMMLanguageBinder languageBinder,
        IMediaPlayer mediaPlayer,
        IGetLyricsLinkAction getLyricsLinkAction,
        ITranscriptionPOFactory transcriptionPOFactory)
    {
        _tracksClient = tracksClient;
        _languageBinder = languageBinder;
        _mediaPlayer = mediaPlayer;
        _getLyricsLinkAction = getLyricsLinkAction;
        _transcriptionPOFactory = transcriptionPOFactory;
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

        var items = new List<IBasePO>();
        
        if (track.IsSong())
            items.Add(new TranscriptionHeaderPO(trackTitle, PrepareSubtitle(track), DataContext.HeaderClickedCommand));

        DataContext.LyricsLink = await _getLyricsLinkAction.ExecuteGuarded(track);
        DataContext.HasTimeframes = response.Last().End != 0;
        DataContext.IsAutoTranscribed = track.Subtype.IsNoneOf(TrackSubType.Singsong, TrackSubType.Song);
        DataContext.HeaderText = GetHeaderText();
        items.AddRange(_transcriptionPOFactory.Create(response, (Track)DataContext.Track, DataContext.HasTimeframes));
        DataContext.Transcriptions.ReplaceWith(items);
    }

    private string GetHeaderText()
    {
        if (DataContext.IsAutoTranscribed)
            return _languageBinder[Translations.HighlightedTextTrackViewModel_AutoTranscribed];

        switch (DataContext.LyricsLink.LyricsLinkType)
        {
            case LyricsLinkType.SongTreasures:
                return _languageBinder[Translations.ReadTranscriptionViewModel_SongTreasures];
            case LyricsLinkType.None:
            case LyricsLinkType.Generic:
                return _languageBinder[Translations.ReadTranscriptionViewModel_Lyrics];
            default:
                return default;
        }
    }

    private async Task TranscriptionClickedAction(Transcription transcription)
    {
        if (!DataContext.HasTimeframes)
            return;

        long startTime = (long)TimeSpan.FromSeconds(transcription.Start).TotalMilliseconds;
        var track = (Track)DataContext.Track;
        
        if (_mediaPlayer.CurrentTrack?.Id != DataContext.Track.Id)
        {
            await _mediaPlayer.Play(track.EncloseInArray(), track, startTime);
            return;
        }

        _mediaPlayer.SeekTo(startTime);
    }

    private string PrepareSubtitle(ITrackModel trackModel)
    {
        var subtitleStringBuilder = new StringBuilder();

        if (!trackModel.Meta.Lyricist.IsNullOrEmpty())
        {
            subtitleStringBuilder.Append($"{_languageBinder[Translations.ReadTranscriptionViewModel_Text]}: {trackModel.Meta.Lyricist}");
        }

        if (!trackModel.Meta.Composer.IsNullOrEmpty())
        {
            subtitleStringBuilder.Append($" {Dot} ");
            subtitleStringBuilder.Append($"{_languageBinder[Translations.ReadTranscriptionViewModel_Melody]}: {trackModel.Meta.Composer}");
        }

        return subtitleStringBuilder.ToString();
    }
}