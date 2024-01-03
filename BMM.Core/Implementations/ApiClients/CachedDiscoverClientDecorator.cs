using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BMM.Api.Abstraction;
using BMM.Api.Implementation.Clients.Contracts;
using BMM.Api.Implementation.Models;
using BMM.Api.Implementation.Models.Enums;
using BMM.Core.Implementations.Caching;

namespace BMM.Core.Implementations.ApiClients
{
    public class CachedDiscoverClientDecorator: IDiscoverClient
    {
        private readonly IDiscoverClient _client;
        private readonly IClientCache _clientCache;

        public CachedDiscoverClientDecorator(IDiscoverClient client, IClientCache clientCache)
        {
            _client = client;
            _clientCache = clientCache;
        }

        public Task<IEnumerable<Document>> GetDocuments(string lang, int? age, AppTheme theme, CachePolicy cachePolicy)
        {
            return _clientCache.Get(
                () => _client.GetDocuments(lang, age, theme, cachePolicy),
                cachePolicy,
                TimeSpan.FromMinutes(30),
                CacheKeys.DiscoverGetDocuments
            );
        }
    }
}