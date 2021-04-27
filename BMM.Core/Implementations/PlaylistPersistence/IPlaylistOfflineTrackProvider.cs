using System.Collections.Generic;
using System.Threading.Tasks;
using BMM.Api.Implementation.Models;

namespace BMM.Core.Implementations.PlaylistPersistence
{
    public interface IPlaylistOfflineTrackProvider
    {
        Task<IEnumerable<Track>> GetCollectionTracksSupposedToBeDownloaded();
    }
}