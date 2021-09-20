using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using Akavache;

namespace BMM.Core.Implementations.Caching
{
    public class LocalStorageCache : ICache
    {
        private readonly IBlobCache _blobCache;

        public LocalStorageCache(IBlobCache blobCache)
        {
            _blobCache = blobCache;
        }

        public async Task Clear()
        {
            var keys = await _blobCache.GetAllKeys();
            foreach (var key in keys.Where(k => k.StartsWith("ClientCache|")))
            {
                await _blobCache.Invalidate(key);
            }
        }

        public Task<CachedItem<T>> GetOrFetchObject<T>(string key, Func<Task<T>> fetchFunc, DateTimeOffset? absoluteExpiration = null)
        {
            return _blobCache.GetOrFetchObject(key,
                    async () => CachedItem<T>.New(await fetchFunc.Invoke()),
                    absoluteExpiration)
                .ToTask();
        }

        public async Task UpdateItem<T>(string key, CachedItem<T> item)
        {
            await _blobCache.InvalidateObject<T>(key);
            await _blobCache.InsertObject(key, item);
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