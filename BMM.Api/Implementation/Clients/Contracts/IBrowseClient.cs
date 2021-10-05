using System.Collections.Generic;
using System.Threading.Tasks;
using BMM.Api.Implementation.Models;

namespace BMM.Api.Implementation.Clients.Contracts
{
    public interface IBrowseClient
    {
        Task<IEnumerable<Document>> Get();
        Task<IEnumerable<Document>> GetEvents(int skip, int take);
        Task<IEnumerable<Document>> GetAudiobooks(int skip, int take);
        Task<IEnumerable<Document>> GetMusic();
        Task<IEnumerable<Document>> GetPodcasts();

    }
}