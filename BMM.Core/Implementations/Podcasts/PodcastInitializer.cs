using System.Reactive.Linq;
using System.Threading.Tasks;
using Akavache;
using BMM.Api.Implementation.Models;
using BMM.Core.Implementations.Security;
using BMM.Core.Implementations.Startup;
using BMM.Core.Implementations.Storage;

namespace BMM.Core.Implementations.Podcasts
{
    public class PodcastInitializer : IDelayedStartupTask
    {
        private readonly IUserAuthChecker _userAuthChecker;
        private readonly IPodcastOfflineManager _podcastOfflineManager;

        public PodcastInitializer(IUserAuthChecker userAuthChecker, IPodcastOfflineManager podcastOfflineManager)
        {
            _userAuthChecker = userAuthChecker;
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

            if (AppSettings.FirstLaunchWithPodcasts)
            {
                var podcast = new Podcast { Id = 1 };

                await _podcastOfflineManager.FollowPodcast(podcast);
                AppSettings.FirstLaunchWithPodcasts = false;
            }
        }
    }
}
