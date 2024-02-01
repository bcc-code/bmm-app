using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using BMM.Api.Abstraction;
using BMM.Api.Implementation.Models;

namespace BMM.Api.Implementation.Clients.Contracts
{
    public interface IPlaylistClient
    {
        Task<IList<Playlist>> GetAll(CachePolicy cachePolicy);

        Task<Playlist> GetById(int id, CachePolicy cachePolicy);

        Task<Stream> GetCover(int podcastId);
        
        Task<GenericDocumentsHolder> GetDocuments(int? age, CachePolicy cachePolicy);

        Task<IList<Track>> GetTracks(int podcastId, CachePolicy cachePolicy);
    }
}