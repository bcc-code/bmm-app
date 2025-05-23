using System.Collections.Generic;
using System.Threading.Tasks;
using BMM.Api.Abstraction;
using BMM.Api.Implementation.Models;
using BMM.Api.Implementation.Models.Enums;

namespace BMM.Api.Implementation.Clients.Contracts
{
    public interface IDiscoverClient
    {
        Task<IEnumerable<Document>> GetDocuments(int? age, AppTheme theme, CachePolicy cachePolicy);
        Task<IEnumerable<Document>> GetDocumentsCarPlay(AppTheme theme, CachePolicy cachePolicy);
    }
}