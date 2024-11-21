using BMM.Api.Abstraction;
using BMM.Api.Implementation.Models;
using BMM.Api.Utils;
using BMM.Core.GuardedActions.Base;
using BMM.Core.GuardedActions.Player.Interfaces;
using BMM.Core.Implementations.FirebaseRemoteConfig;
using BMM.Core.Models.Player.Lyrics;

namespace BMM.Core.GuardedActions.Player;

public class GetLyricsLinkAction
    : GuardedActionWithParameterAndResult<ITrackModel, LyricsLink>,
      IGetLyricsLinkAction
{
    private const string SangtekstRelationName = "Sangtekst";
    private readonly IFirebaseRemoteConfig _firebaseRemoteConfig;

    public GetLyricsLinkAction(IFirebaseRemoteConfig firebaseRemoteConfig)
    {
        _firebaseRemoteConfig = firebaseRemoteConfig;
    }
    
    protected override async Task<LyricsLink> Execute(ITrackModel track)
    {
        var existingSongbook = track
            ?.Relations
            ?.OfType<TrackRelationSongbook>()
            .FirstOrDefault();

        if (existingSongbook != null)
        {
            string link = string.Format(_firebaseRemoteConfig.SongTreasuresSongLink,
                SongbookUtils.GetShortName(existingSongbook.Name),
                existingSongbook.Id);

            return new LyricsLink(LyricsLinkType.SongTreasures, link);
        }

        var sangtekstElement = track
            ?.Relations
            ?.OfType<TrackRelationExternal>()
            .FirstOrDefault(x => string.Equals(x.Name, SangtekstRelationName, StringComparison.OrdinalIgnoreCase));

        if (sangtekstElement == null)
            return LyricsLink.Empty;

        return new LyricsLink(LyricsLinkType.Generic, sangtekstElement.Url);
    }
}