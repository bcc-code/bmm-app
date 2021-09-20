using System.Reactive.Linq;
using System.Threading.Tasks;
using Akavache;
using BMM.Core.Implementations.Persistence.Interfaces;

namespace BMM.Core.Implementations.Persistence
{
    public class BlobCacheWrapper : IBlobCacheWrapper
    {
        private readonly IBlobCache _blobCache;

        public BlobCacheWrapper(IBlobCache blobCache)
        {
            _blobCache = blobCache;
        }

        public async Task<T> GetObject<T>(string key)
        {
            return await _blobCache.GetObject<T>(key);
        }

        public async Task InsertObject<T>(string key, T obj)
        {
            await _blobCache.InsertObject(key, obj);
        }
    }
}