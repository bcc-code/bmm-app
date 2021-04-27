using System.Threading.Tasks;
using BMM.Api.Implementation.Models;

namespace BMM.Core.Implementations.PlaylistPersistence
{
    public interface IPlaylistManager
    {
        Task DownloadPlaylist(Playlist playlist);

        Task RemoveDownloadedPlaylist(Playlist playlist);
    }
}