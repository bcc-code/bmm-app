using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BMM.Api.Abstraction;
using BMM.Core.Implementations.Caching;
using BMM.Core.Implementations.Security;
using BMM.Core.Messages;
using MvvmCross.Plugin.Messenger;

namespace BMM.Core.Implementations.ApiClients
{
    public interface IClientCache
    {
        Task<T> Get<T>(Func<Task<T>> actionIfNotCached, CachePolicy cachePolicy, TimeSpan maxAge, CacheKeys key, params string[] keyParameters);

        Task<IList<T>> GetLoadMoreItems<T>(Func<int, int, Task<IList<T>>> actionIfNotCached, int size, int @from, CachePolicy cachePolicy, TimeSpan maxAge, CacheKeys key,
            params string[] keyParameters);
    }

    public class ClientCache : IClientCache
    {
        private readonly ICache _cache;
        private readonly IUserStorage _userStorage;
        private readonly IMvxMessenger _messenger;

        public ClientCache(ICache cache, IUserStorage userStorage, IMvxMessenger messenger)
        {
            _cache = cache;
            _userStorage = userStorage;
            _messenger = messenger;
        }

        public async Task<T> Get<T>(Func<Task<T>> actionIfNotCached, CachePolicy cachePolicy, TimeSpan maxAge, CacheKeys key, params string[] keyParameters)
        {
            var cacheParts = new List<string>
            {
                "ClientCache",
                _userStorage.GetUser().Username,
                key.ToString()
            };
            // ToDo: maybe we should add the language here as well
            cacheParts.AddRange(keyParameters);

            var cacheId = string.Join("|", cacheParts);

            CachedItem<T> item;
            if (cachePolicy == CachePolicy.BypassCache)
            {
                item = await FetchAndUpdateCache(actionIfNotCached, cacheId);
            }
            else
            {
                item = await _cache.GetOrFetchObject(cacheId, actionIfNotCached);
            }

            bool isCachedItemOutdated = item.DateCreated + maxAge < DateTime.UtcNow;
            if (isCachedItemOutdated && cachePolicy == CachePolicy.UseCacheAndRefreshOutdated)
            {
                _messenger.Publish(new BackgroundTaskMessage(this,
                    async () =>
                    {
                        await FetchAndUpdateCache(actionIfNotCached, cacheId);
                        _messenger.Publish(new CacheUpdatedMessage(this, key));

                        // ToDo: do something with keyParameters
                    }));
            }

            if (isCachedItemOutdated && cachePolicy == CachePolicy.UseCacheAndWaitForUpdates)
            {
                item = await FetchAndUpdateCache(actionIfNotCached, cacheId);
            }

            return item.Item;
        }

        public async Task<IList<T>> GetLoadMoreItems<T>(Func<int, int, Task<IList<T>>> actionIfNotCached, int size, int @from, CachePolicy cachePolicy, TimeSpan maxAge,
            CacheKeys key, params string[] keyParameters)
        {
            IList<T> items;
            if (@from == 0)
                items = await Get(() => actionIfNotCached.Invoke(size, @from), cachePolicy, maxAge, key, keyParameters);
            else
                // Only use the cache for the first items but not if you load more
                items = await actionIfNotCached.Invoke(size, @from);

            return items;
        }

        private async Task<CachedItem<T>> FetchAndUpdateCache<T>(Func<Task<T>> actionIfNotCached, string cacheKey)
        {
            var value = await actionIfNotCached.Invoke();
            var item = CachedItem<T>.New(value);
            await _cache.UpdateItem(cacheKey, item);
            return item;
        }
    }
}
