using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BMM.Api.Abstraction;
using BMM.Api.Implementation.Clients.Contracts;
using BMM.Api.Implementation.Models;
using BMM.Core.Implementations.Caching;

namespace BMM.Core.Implementations.ApiClients
{
    public class CachedBrowseClientDecorator : IBrowseClient
    {
        private readonly IBrowseClient _browseClient;
        private readonly IClientCache _clientCache;

        public CachedBrowseClientDecorator(IBrowseClient browseClient, IClientCache clientCache)
        {
            _browseClient = browseClient;
            _clientCache = clientCache;
        }

        public Task<IEnumerable<Document>> Get(CachePolicy policy)
        {
            return _clientCache.Get(
                () => _browseClient.Get(policy),
                policy,
                TimeSpan.FromHours(1),
                CacheKeys.BrowseGet
            );
        }

        public Task<GenericDocumentsHolder> GetDocuments(string path, int skip, int take)
        {
            return _browseClient.GetDocuments(path, skip, take);
        }
    }
}