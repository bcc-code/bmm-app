using System.Collections.Generic;
using System.Threading.Tasks;
using BMM.Api.Implementation.Models;

namespace BMM.Core.Implementations.TrackCollections
{
    public interface ITrackCollectionOfflineTrackProvider
    {
        Task<IEnumerable<Track>> GetCollectionTracksSupposedToBeDownloaded();
    }
}