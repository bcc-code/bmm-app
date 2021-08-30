using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using BMM.Api;
using BMM.Api.Abstraction;
using BMM.Api.Implementation.Clients.Contracts;
using BMM.Api.Implementation.Models;
using BMM.Core.Implementations.Caching;

namespace BMM.Core.Implementations.ApiClients
{
    public class CachedContributorClientDecorator: IContributorClient
    {
        private readonly IContributorClient _client;
        private readonly IClientCache _clientCache;

        public CachedContributorClientDecorator(IContributorClient client, IClientCache clientCache)
        {
            _client = client;
            _clientCache = clientCache;
        }

        public Task<int> Add(Contributor contributor)
        {
            return _client.Add(contributor);
        }

        public Task<IList<Contributor>> GetAll(int size = ApiConstants.LoadMoreSize, int @from = 0, string orderBy = null)
        {
            return _client.GetAll(size, from, orderBy);
        }

        public Task<Contributor> GetById(int id)
        {
            return _clientCache.Get(
                () => _client.GetById(id),
                CachePolicy.UseCacheAndRefreshOutdated,
                TimeSpan.FromHours(1),
                CacheKeys.ContributorGetById,
                "id:" + id);
        }

        public Task<IList<Contributor>> GetByTerm(string term, int size = ApiConstants.LoadMoreSize)
        {
            return _client.GetByTerm(term, size);
        }

        public Task<Stream> GetCover(int contributorId)
        {
            return _client.GetCover(contributorId);
        }

        public Task<IList<Track>> GetTracks(
            int contributorId,
            CachePolicy cachePolicy,
            int size = ApiConstants.LoadMoreSize,
            int from = 0,
            IEnumerable<string> contentTypes = null)
        {
            return _clientCache.GetLoadMoreItems(
                (s, f) => _client.GetTracks(contributorId, cachePolicy, s, f, contentTypes),
                size,
                from,
                cachePolicy,
                TimeSpan.FromHours(1),
                CacheKeys.ContributorGetTracks,
                "contentTypes:" + (contentTypes == null ? "null" : string.Join(",", contentTypes)),
                "id" + contributorId
            );
        }
    }
}