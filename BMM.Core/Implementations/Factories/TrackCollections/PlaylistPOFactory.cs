using BMM.Api.Implementation.Models;
using BMM.Core.Models.POs.Playlists;

namespace BMM.Core.Implementations.Factories.TrackCollections;

public class PlaylistPOFactory : IPlaylistPOFactory
{
    public PlaylistPO Create(Playlist playlist)
    {
        return new PlaylistPO(playlist);
    }
}