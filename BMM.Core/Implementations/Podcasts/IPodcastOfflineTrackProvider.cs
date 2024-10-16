using System.Collections.Generic;
using System.Threading.Tasks;
using BMM.Api.Implementation.Models;
using BMM.Core.Implementations.Downloading;

namespace BMM.Core.Implementations.Podcasts
{
    public interface IPodcastOfflineTrackProvider : IOfflineTrackProvider
    {
        Task<ICollection<int>> GetFollowedPodcasts();
    }
}