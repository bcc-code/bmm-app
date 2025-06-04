using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BMM.Api;
using BMM.Api.Abstraction;
using BMM.Api.Implementation.Clients.Contracts;
using BMM.Api.Implementation.Models;
using BMM.Core.Implementations.Caching;

namespace BMM.Core.Implementations.ApiClients
{
    public class CachedTracksClientDecorator : ITracksClient
    {
        private readonly ITracksClient _client;
        private readonly IClientCache _clientCache;

        public CachedTracksClientDecorator(ITracksClient client, IClientCache clientCache)
        {
            _client = client;
            _clientCache = clientCache;
        }

        public Task<IList<Track>> GetAll(CachePolicy cachePolicy, int size = ApiConstants.LoadMoreSize, int @from = 0, IEnumerable<TrackSubType> contentTypes = null,
            IEnumerable<string> tags = null, IEnumerable<string> excludeTags = null)
        {
            return _clientCache.GetLoadMoreItems((s, f) => _client.GetAll(cachePolicy, size: s, @from: f, contentTypes: contentTypes, tags: tags, excludeTags: excludeTags),
                size,
                @from,
                cachePolicy,
                TimeSpan.FromHours(1),
                CacheKeys.TracksGetAll,
                "contentTypes:" + (contentTypes == null ? "null" : string.Join(",", contentTypes)),
                "tags:" + (tags == null ? "null" : string.Join(",", tags)),
                "excludeTags:" + (excludeTags == null ? "null" : string.Join(",", excludeTags)));
        }

        public Task<Track> GetById(int id, string desiredLanguage = default) => _client.GetById(id, desiredLanguage);

        public Task<IList<Track>> GetRecommendations() => _client.GetRecommendations();
        public Task<IList<Track>> GetRecommendationsAfterFraKaare() => _client.GetRecommendationsAfterFraKaare();
        public Task<IList<Transcription>> GetTranscriptions(int trackId) => _client.GetTranscriptions(trackId);
    }
}
