using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BMM.Api.Abstraction;
using BMM.Api.Implementation.Clients.Contracts;
using BMM.Api.Implementation.Models;
using BMM.Core.Implementations.Caching;

namespace BMM.Core.Implementations.ApiClients
{
    public class CachedTrackCollectionClientDecorator : ITrackCollectionClient
    {
        private readonly ITrackCollectionClient _client;
        private readonly IClientCache _clientCache;

        public CachedTrackCollectionClientDecorator(ITrackCollectionClient client, IClientCache clientCache)
        {
            _client = client;
            _clientCache = clientCache;
        }

        public Task<bool> AddTracksToTrackCollection(int id, IList<int> trackIds)
        {
            return _client.AddTracksToTrackCollection(id, trackIds);
        }

        public Task AddAlbumToTrackCollection(int id, int albumId)
        {
            return _client.AddAlbumToTrackCollection(id, albumId);
        }

        public Task<bool> Delete(int id)
        {
            return _client.Delete(id);
        }

        public Task<IList<TrackCollection>> GetAll(CachePolicy cachePolicy)
        {
            return _clientCache.Get(() => _client.GetAll(cachePolicy), cachePolicy, TimeSpan.FromHours(0), CacheKeys.TrackCollectionGetAll);
        }

        public Task<TrackCollection> GetById(int id, CachePolicy cachePolicy)
        {
            return _clientCache.Get(() => _client.GetById(id, cachePolicy), cachePolicy, TimeSpan.FromHours(0), CacheKeys.TrackCollectionGetById, id.ToString());
        }

        public Task Save(TrackCollection collection)
        {
            return _client.Save(collection);
        }

        public Task<int> Create(TrackCollection collection)
        {
            return _client.Create(collection);
        }

        public Task<bool> ResetShare(int id)
        {
            return _client.ResetShare(id);
        }

        public Task<bool> Unfollow(int id)
        {
            return _client.Unfollow(id);
        }

        public Task<TopSongsCollection> GetTopSongs()
        {
            return _client.GetTopSongs();
        }

        public Task AddTopSongsToFavourites()
        {
            return _client.AddTopSongsToFavourites();
        }
    }
}
