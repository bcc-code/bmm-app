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
        private readonly IBlobCache _cache;

        public LocalStorageCache(IBlobCache cache)
        {
            _cache = cache;
        }

        public async Task Clear()
        {
            var keys = await _cache.GetAllKeys();
            foreach (var key in keys.Where(k => k.StartsWith("ClientCache|")))
            {
                await _cache.Invalidate(key);
            }
        }

        public Task<CachedItem<T>> GetOrFetchObject<T>(string key, Func<Task<T>> fetchFunc, DateTimeOffset? absoluteExpiration = null)
        {
            return _cache.GetOrFetchObject(key,
                    async () => CachedItem<T>.New(await fetchFunc.Invoke()),
                    absoluteExpiration)
                .ToTask();
        }

        public async Task UpdateItem<T>(string key, CachedItem<T> item)
        {
            await _cache.InvalidateObject<T>(key);
            await _cache.InsertObject(key, item);
        }
    }
}