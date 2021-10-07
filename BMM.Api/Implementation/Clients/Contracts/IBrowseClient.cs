using System.Collections.Generic;
using System.Threading.Tasks;
using BMM.Api.Implementation.Models;

namespace BMM.Api.Implementation.Clients.Contracts
{
    public interface IBrowseClient
    {
        Task<IEnumerable<Document>> Get();
        Task<GenericDocumentsHolder> GetDocuments(string path, int skip, int take);
    }
}