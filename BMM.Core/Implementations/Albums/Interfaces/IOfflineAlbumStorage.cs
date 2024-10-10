namespace BMM.Core.Implementations.Albums.Interfaces;

public interface IOfflineAlbumStorage
{
    void Add(int id);
    void Delete(int id);
    bool IsOfflineAvailable(int id);
    HashSet<int> GetAlbumIds();
}