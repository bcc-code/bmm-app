using System.Reactive.Linq;
using System.Threading.Tasks;
using Akavache;
using BMM.Api.Implementation.Models;
using BMM.Core.Implementations.Security;
using BMM.Core.Implementations.Startup;

namespace BMM.Core.Implementations.Podcasts
{
    public class PodcastInitializer : IDelayedStartupTask
    {
        private const string FirstLaunchWithPodcastsKey = "first_launch_with_podcasts";
        private readonly IUserAuthChecker _userAuthChecker;
        private readonly IBlobCache _blobCache;
        private readonly IPodcastOfflineManager _podcastOfflineManager;

        public PodcastInitializer(IUserAuthChecker userAuthChecker, IBlobCache blobCache, IPodcastOfflineManager podcastOfflineManager)
        {
            _userAuthChecker = userAuthChecker;
            _blobCache = blobCache;
            _podcastOfflineManager = podcastOfflineManager;
        }

        public async Task RunAfterStartup()
        {
            if (await _userAuthChecker.IsUserAuthenticated())
            {
                await InitializePodcasts();
            }
        }

        private async Task InitializePodcasts()
        {
            await _podcastOfflineManager.InitAsync();

            var firstLaunchWithPodcasts = await _blobCache.GetOrCreateObject(FirstLaunchWithPodcastsKey, () => true);

            if (firstLaunchWithPodcasts)
            {
                var podcast = new Podcast { Id = 1 };

                await _podcastOfflineManager.FollowPodcast(podcast);
                await _blobCache.InsertObject(FirstLaunchWithPodcastsKey, false);
            }
        }
    }
}
