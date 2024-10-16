using BMM.Api.Implementation.Models;

namespace BMM.Core.Implementations.Downloading;

public interface IOfflineTrackProvider
{
    Task<IList<Track>> GetTracksSupposedToBeDownloaded();
}