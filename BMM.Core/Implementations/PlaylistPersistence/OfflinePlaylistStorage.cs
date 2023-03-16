using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Akavache;
using BMM.Core.Helpers;
using BMM.Core.Implementations.Storage;

namespace BMM.Core.Implementations.PlaylistPersistence
{
    public class OfflinePlaylistStorage : IOfflinePlaylistStorage
    {
        public async Task Add(int id)
        {
            var ids = await GetPlaylistIds();
            ids.Add(id);
            AppSettings.LocalPlaylists = ids;
        }

        public async Task Delete(int id)
        {
            var ids = await GetPlaylistIds();
            ids.Remove(id);
            AppSettings.LocalPlaylists = ids;
        }

        public async Task<bool> IsOfflineAvailable(int id)
        {
            var ids = await GetPlaylistIds();
            return ids.Contains(id);
        }

        public async Task<HashSet<int>> GetPlaylistIds() => AppSettings.LocalPlaylists;
    }
}