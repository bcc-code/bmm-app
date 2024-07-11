using BMM.Api.Implementation.Models;
using BMM.Core.Models.POs.Playlists;

namespace BMM.Core.Implementations.Factories.TrackCollections;

public interface IPlaylistPOFactory
{
    PlaylistPO Create(Playlist playlist);
}