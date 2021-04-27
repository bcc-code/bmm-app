using System.Threading.Tasks;
using BMM.Core.Implementations.Downloading;

namespace BMM.Core.Implementations.PlaylistPersistence
{
    public class PlaylistManager : IPlaylistManager
    {
        private readonly IOfflinePlaylistStorage _offlinePlaylistStorage;
        private readonly IGlobalMediaDownloader _globalMediaDownloader;

        public PlaylistManager(IOfflinePlaylistStorage offlinePlaylistStorage, IGlobalMediaDownloader globalMediaDownloader)
        {
            _offlinePlaylistStorage = offlinePlaylistStorage;
            _globalMediaDownloader = globalMediaDownloader;
        }

        public async Task DownloadPlaylist(Api.Implementation.Models.Playlist playlist)
        {
            await _offlinePlaylistStorage.Add(playlist.Id);
            await _globalMediaDownloader.SynchronizeOfflineTracks();
        }

        public async Task RemoveDownloadedPlaylist(Api.Implementation.Models.Playlist playlist)
        {
            await _offlinePlaylistStorage.Delete(playlist.Id);
            await _globalMediaDownloader.SynchronizeOfflineTracks();
        }
    }
}