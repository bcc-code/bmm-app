using System.Threading.Tasks;
using BMM.Api.Implementation.Models;

namespace BMM.Core.Implementations.TrackCollections
{
    public interface ITrackCollectionManager
    {
        Task AddToTrackCollection(TrackCollection trackCollection, int id, DocumentType type, string origin);
        Task DownloadTrackCollection(TrackCollection trackCollection);
        Task RemoveDownloadedTrackCollection(TrackCollection trackCollection);
        bool IsOfflineAvailable(TrackCollection trackCollection);
    }
}