using System.Collections.Generic;
using System.Threading.Tasks;
using BMM.Api.Implementation.Models;

namespace BMM.Core.Implementations.Podcasts
{
    public interface IPodcastOfflineManager
    {
        bool IsFollowing(Podcast podcast);
        Task FollowPodcast(Podcast podcast);
        Task UnfollowPodcast(Podcast podcast);
        int GetNumberOfTracksToAutomaticallyDownload(Podcast podcast);
        void SetNumbeOfTracksToAutomaticallyDownload(Podcast podcast, int numberOfTracks);
        ICollection<int> GetPodcastsFollowing();
        Task InitAsync();
        Task Clear();
    }
}