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
        private const int MaxGlobalCacheDurationInMonths = 1;
        private const string CachePrefix = "ClientCache";
        
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
            if (cachePolicy == CachePolicy.IgnoreCache)
                return await actionIfNotCached.Invoke();

            string cacheId = BuildCacheId(key, keyParameters);

            if (cachePolicy == CachePolicy.ForceGetAndUpdateCache)
                return await FetchAndUpdateCache(actionIfNotCached, cacheId);

            var cacheItem = await _cache.GetOrFetchObject(cacheId, actionIfNotCached);
            bool isCachedItemOutdated = cacheItem.DateCreated + maxAge < DateTime.UtcNow;
            
            if (!isCachedItemOutdated)
                return cacheItem.Item;
            
            bool isMaximumCacheTimeExceeded = cacheItem.DateCreated < DateTime.UtcNow.AddMonths(-MaxGlobalCacheDurationInMonths);

            if (cachePolicy == CachePolicy.UseCacheAndWaitForUpdates || isMaximumCacheTimeExceeded)
                return await FetchAndUpdateCache(actionIfNotCached, cacheId);

            if (cachePolicy == CachePolicy.UseCacheAndRefreshOutdated)
                PublishBackgroundTaskMessage(actionIfNotCached, cacheId, key);
            
            return cacheItem.Item;
        }

        private void PublishBackgroundTaskMessage<T>(
            Func<Task<T>> actionIfNotCached,
            string cacheId,
            CacheKeys key)
        {
            _messenger.Publish(new BackgroundTaskMessage(this,
                async () =>
                {
                    await FetchAndUpdateCache(actionIfNotCached, cacheId);
                    _messenger.Publish(new CacheUpdatedMessage(this, key));

                    // ToDo: do something with keyParameters
                }));
        }

        public async Task<IList<T>> GetLoadMoreItems<T>(
            Func<int, int, Task<IList<T>>> actionIfNotCached,
            int size,
            int @from,
            CachePolicy cachePolicy,
            TimeSpan maxAge,
            CacheKeys key,
            params string[] keyParameters)
        {
            return await Get(() => actionIfNotCached.Invoke(size, @from), cachePolicy, maxAge, key, keyParameters);
        }

        private async Task<T> FetchAndUpdateCache<T>(Func<Task<T>> actionIfNotCached, string cacheKey)
        {
            var value = await actionIfNotCached.Invoke();
            var cacheItem = CachedItem<T>.New(value);
            await _cache.UpdateItem(cacheKey, cacheItem);
            return cacheItem.Item;
        }

        private string BuildCacheId(CacheKeys key, IEnumerable<string> keyParameters)
        {
            var cacheParts = new List<string>
            {
                CachePrefix,
                _userStorage.GetUser().Username,
                key.ToString()
            };
            
            cacheParts.AddRange(keyParameters);
            return string.Join("|", cacheParts);
        }
    }
}
