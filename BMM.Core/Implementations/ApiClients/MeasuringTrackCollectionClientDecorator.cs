﻿using BMM.Api.Abstraction;
using BMM.Api.Implementation.Clients.Contracts;
using BMM.Api.Implementation.Models;
using BMM.Core.Implementations.Analytics;

namespace BMM.Core.Implementations.ApiClients
{
    public class MeasuringTrackCollectionClientDecorator : ITrackCollectionClient
    {
        private readonly ITrackCollectionClient _client;
        private readonly IStopwatchManager _stopwatchManager;

        public MeasuringTrackCollectionClientDecorator(ITrackCollectionClient client, IClientCache clientCache, IStopwatchManager stopwatchManager)
        {
            _client = client;
            _stopwatchManager = stopwatchManager;
        }

        public Task<bool> AddTracksToTrackCollection(int targetId, IList<int> trackIds)
        {
            return _client.AddTracksToTrackCollection(targetId, trackIds);
        }

        public Task AddAlbumToTrackCollection(int targetId, int albumId)
        {
            return _client.AddAlbumToTrackCollection(targetId, albumId);
        }

        public Task AddPlaylistToTrackCollection(int targetId, int playlistId)
        {
            return _client.AddPlaylistToTrackCollection(targetId, playlistId);
        }

        public Task AddTrackCollectionToTrackCollection(int targetId, int trackCollectionId)
        {
            return _client.AddTrackCollectionToTrackCollection(targetId, trackCollectionId);
        }

        public Task<bool> Delete(int id)
        {
            return _client.Delete(id);
        }

        public async Task<IList<TrackCollection>> GetAll(CachePolicy cachePolicy)
        {
            var watch = _stopwatchManager.StartAndGetStopwatch(StopwatchType.TrackCollectionAll);
            var result = await _client.GetAll(cachePolicy);
            watch.Stop();
            return result;
        }

        public async Task<TrackCollection> GetById(int id, CachePolicy cachePolicy)
        {
            var watch = _stopwatchManager.StartAndGetStopwatch(StopwatchType.TrackCollectionSingle);
            var result = await _client.GetById(id, cachePolicy);
            watch.Stop();
            return result;
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

        public Task<bool> Like(IList<int> trackIds)
        {
            return _client.Like(trackIds);
        }

        public Task<bool> Unlike(IList<int> trackIds)
        {
            return _client.Unlike(trackIds);
        }
    }
}
