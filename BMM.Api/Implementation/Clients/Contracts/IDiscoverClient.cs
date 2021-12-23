using System.Collections.Generic;
using System.Threading.Tasks;
using BMM.Api.Abstraction;
using BMM.Api.Implementation.Models;

namespace BMM.Api.Implementation.Clients.Contracts
{
    public interface IDiscoverClient
    {
        Task<IEnumerable<Document>> GetDocuments(string lang, CachePolicy cachePolicy);
    }
}