using BMM.Api.Implementation.Models;
using BMM.Api.Implementation.Models.Interfaces;
using BMM.Core.Models.POs.Albums;
using BMM.Core.Models.POs.Base;
using BMM.Core.Models.POs.Base.Interfaces;
using BMM.Core.Models.POs.Contributors;
using BMM.Core.Models.POs.Playlists;
using BMM.Core.Models.POs.Tiles;
using BMM.Core.Models.POs.Tracks;

namespace BMM.Core.Extensions;

public static class DocumentExtensions
{
    public static string GetCoverUrl(this Document document)
    {
        return document switch
        {
            Track track => track.ArtworkUri,
            ITrackListDisplayable trackListDisplayable => trackListDisplayable.Cover,
            ContinueListeningTile continueListeningTile => continueListeningTile.CoverUrl,
            Contributor contributor => contributor.Cover,
            _ => null
        };
    }
    
    public static string GetCoverUrl(this IDocumentPO documentPO)
    {
        return documentPO switch
        {
            TrackPO track => track.Track.GetCoverUrl(),
            ITrackListHolderPO trackListHolderPO => trackListHolderPO.Cover,
            ContinueListeningTilePO continueListeningTilePO => continueListeningTilePO.Tile.GetCoverUrl(),
            ContributorPO contributorPO => contributorPO.Contributor.GetCoverUrl(),
            _ => null
        };
    }
}