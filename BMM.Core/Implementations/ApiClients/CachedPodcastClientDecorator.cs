using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BMM.Api;
using BMM.Api.Abstraction;
using BMM.Api.Implementation.Clients.Contracts;
using BMM.Api.Implementation.Models;
using BMM.Core.Implementations.Caching;

namespace BMM.Core.Implementations.ApiClients
{
    public class CachedPodcastClientDecorator : IPodcastClient
    {
        private readonly IPodcastClient _client;
        private readonly IClientCache _clientCache;

        public CachedPodcastClientDecorator(IPodcastClient client, IClientCache clientCache)
        {
            _client = client;
            _clientCache = clientCache;
        }

        public Task<IList<Podcast>> GetAll(CachePolicy cachePolicy)
        {
            return _clientCache.Get(() => _client.GetAll(cachePolicy), cachePolicy, TimeSpan.FromHours(24), CacheKeys.PodcastGetAll);
        }

        public async Task<Podcast> GetById(int id, CachePolicy cachePolicy)
        {
            var all = await GetAll(cachePolicy);

            if (all.All(p => p.Id != id))
            {
                all = await GetAll(CachePolicy.ForceGetAndUpdateCache);
            }

            return all.First(p => p.Id == id);
        }

        public Task<Stream> GetCover(int podcastId)
        {
            return _client.GetCover(podcastId);
        }

        public Task<IList<Track>> GetTracks(int podcastId, CachePolicy cachePolicy, int size = ApiConstants.LoadMoreSize, int from = 0)
        {
            return _clientCache.GetLoadMoreItems((s, f) => _client.GetTracks(podcastId, cachePolicy, s, f),
                size,
                @from,
                cachePolicy,
                TimeSpan.FromHours(1),
                CacheKeys.PodcastGetTracks,
                podcastId.ToString());
        }

        public Task<Track> GetRandomTrack(int podcastId)
        {
            return _client.GetRandomTrack(podcastId);
        }
    }
}
