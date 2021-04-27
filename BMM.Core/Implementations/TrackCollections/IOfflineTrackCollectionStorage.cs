using System.Collections.Generic;
using System.Threading.Tasks;
using BMM.Api.Implementation.Models;

namespace BMM.Core.Implementations.TrackCollections
{
    public interface IOfflineTrackCollectionStorage
    {
        Task InitAsync();
        bool IsOfflineAvailable(TrackCollection trackCollection);
        Task Add(int id);
        ICollection<int> GetOfflineTrackCollectionIds();
        Task Remove(int id);
        Task Clear();
    }
}