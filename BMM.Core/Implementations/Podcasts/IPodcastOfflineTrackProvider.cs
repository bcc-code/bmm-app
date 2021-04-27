using System.Collections.Generic;
using System.Threading.Tasks;
using BMM.Api.Implementation.Models;

namespace BMM.Core.Implementations.Podcasts
{
    public interface IPodcastOfflineTrackProvider
    {
        Task<IList<Track>> GetPodcastTracksSupposedToBeDownloaded();

        Task<ICollection<int>> GetFollowedPodcasts();
    }
}