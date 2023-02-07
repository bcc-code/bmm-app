using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BMM.Api.Abstraction;
using BMM.Api.Implementation.Clients.Contracts;
using BMM.Api.Implementation.Models;
using BMM.Core.Implementations.Caching;

namespace BMM.Core.Implementations.ApiClients
{
    public class CachedPlaylistClientDecorator : IPlaylistClient
    {
        private readonly IPlaylistClient _client;
        private readonly IClientCache _clientCache;

        public CachedPlaylistClientDecorator(IPlaylistClient client, IClientCache clientCache)
        {
            _client = client;
            _clientCache = clientCache;
        }

        public Task<IList<Playlist>> GetAll(CachePolicy cachePolicy)
        {
            return _clientCache.Get(() => _client.GetAll(cachePolicy), cachePolicy, TimeSpan.FromHours(24), CacheKeys.PlaylistGetAll);
        }

        public async Task<Playlist> GetById(int id, CachePolicy cachePolicy)
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

        public Task<GenericDocumentsHolder> GetDocuments(string lang, int? age, CachePolicy cachePolicy)
        {
            return _clientCache.Get(() => 
                _client.GetDocuments(lang, age, cachePolicy),
                cachePolicy,
                TimeSpan.FromHours(1),
                CacheKeys.PlaylistGetDocument);
        }

        public Task<IList<Track>> GetTracks(int podcastId, CachePolicy cachePolicy)
        {
            return _clientCache.Get(() => _client.GetTracks(podcastId, cachePolicy),
                cachePolicy,
                TimeSpan.FromHours(1),
                CacheKeys.PlaylistGetTracks,
                podcastId.ToString());
        }
    }
}