using BMM.Core.Implementations.Albums.Interfaces;
using BMM.Core.Implementations.Storage;

namespace BMM.Core.Implementations.Albums;

public class OfflineAlbumStorage : IOfflineAlbumStorage
{
    public void Add(int id)
    {
        var ids = GetAlbumIds();
        ids.Add(id);
        AppSettings.LocalAlbums = ids;
    }

    public void Delete(int id)
    {
        var ids = GetAlbumIds();
        ids.Remove(id);
        AppSettings.LocalAlbums = ids;
    }

    public bool IsOfflineAvailable(int id)
    {
        return GetAlbumIds().Contains(id);
    }

    public HashSet<int> GetAlbumIds() => AppSettings.LocalAlbums;
}