using BMM.Api.Implementation.Models;

namespace BMM.Core.Implementations.Albums;

public interface IAlbumManager
{
    Task DownloadAlbum(Album album);
    Task RemoveDownloadedAlbum(Album album);
}