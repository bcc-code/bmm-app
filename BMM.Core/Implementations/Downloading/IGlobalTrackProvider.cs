using System.Collections.Generic;
using System.Threading.Tasks;
using BMM.Api.Implementation.Models;

namespace BMM.Core.Implementations.Downloading
{
    public interface IGlobalTrackProvider
    {
        Task<IEnumerable<Track>> GetTracksSupposedToBeDownloaded();
    }
}