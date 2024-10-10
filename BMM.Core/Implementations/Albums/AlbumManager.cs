using BMM.Api.Implementation.Models;
using BMM.Core.Implementations.Albums.Interfaces;
using BMM.Core.Implementations.Downloading;

namespace BMM.Core.Implementations.Albums;

public class AlbumManager : IAlbumManager
{
    private readonly IOfflineAlbumStorage _offlineAlbumStorage;
    private readonly IGlobalMediaDownloader _globalMediaDownloader;

    public AlbumManager(
        IOfflineAlbumStorage offlineAlbumStorage,
        IGlobalMediaDownloader globalMediaDownloader)
    {
        _offlineAlbumStorage = offlineAlbumStorage;
        _globalMediaDownloader = globalMediaDownloader;
    }
    
    public async Task DownloadAlbum(Album album)
    {
        _offlineAlbumStorage.Add(album.Id);
        await _globalMediaDownloader.SynchronizeOfflineTracks();
    }

    public async Task RemoveDownloadedAlbum(Album album)
    {
        _offlineAlbumStorage.Delete(album.Id);
        await _globalMediaDownloader.SynchronizeOfflineTracks();
    }
}