using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Akavache;
using BMM.Core.Helpers;

namespace BMM.Core.Implementations.PlaylistPersistence
{
    public class OfflinePlaylistStorage : IOfflinePlaylistStorage
    {
        private readonly IBlobCache _blobCache;

        public OfflinePlaylistStorage(IBlobCache blobCache)
        {
            _blobCache = blobCache;
        }

        public async Task Add(int id)
        {
            var ids = await GetPlaylistIds();
            ids.Add(id);
            await _blobCache.InsertObject(StorageKeys.LocalPlaylists, ids, null);
        }

        public async Task Delete(int id)
        {
            var ids = await GetPlaylistIds();
            ids.Remove(id);
            await _blobCache.InsertObject(StorageKeys.LocalPlaylists, ids, null);
        }

        public async Task<bool> IsOfflineAvailable(int id)
        {
            var ids = await GetPlaylistIds();
            return ids.Contains(id);
        }

        public async Task<HashSet<int>> GetPlaylistIds()
        {
            var ids = await _blobCache.GetOrCreateObject(
                StorageKeys.LocalPlaylists,
                () => new HashSet<int>(),
                null);
            return ids;
        }
    }
}