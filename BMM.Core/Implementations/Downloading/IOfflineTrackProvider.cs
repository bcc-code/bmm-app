using BMM.Api.Implementation.Models;

namespace BMM.Core.Implementations.Downloading;

public interface IOfflineTrackProvider
{
    Task<OfflineTracksResult> GetTracksSupposedToBeDownloaded();
}