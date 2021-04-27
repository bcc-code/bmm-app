using System.Collections.Generic;
using System.Threading.Tasks;

namespace BMM.Core.Implementations.PlaylistPersistence
{
    public interface IOfflinePlaylistStorage
    {
        Task Add(int id);

        Task Delete(int id);

        Task<bool> IsOfflineAvailable(int id);

        Task<HashSet<int>> GetPlaylistIds();
    }
}