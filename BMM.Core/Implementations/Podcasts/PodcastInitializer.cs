using BMM.Api.Implementation.Models;
using BMM.Core.Implementations.FirebaseRemoteConfig;
using BMM.Core.Implementations.Security;
using BMM.Core.Implementations.Startup;
using BMM.Core.Implementations.Storage;

namespace BMM.Core.Implementations.Podcasts
{
    public class PodcastInitializer : IDelayedStartupTask
    {
        private readonly IUserAuthChecker _userAuthChecker;
        private readonly IPodcastOfflineManager _podcastOfflineManager;
        private readonly IFirebaseRemoteConfig _config;

        public PodcastInitializer(IUserAuthChecker userAuthChecker, IPodcastOfflineManager podcastOfflineManager, IFirebaseRemoteConfig config)
        {
            _userAuthChecker = userAuthChecker;
            _podcastOfflineManager = podcastOfflineManager;
            _config = config;
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

            foreach (int podcastId in _config.AutoSubscribePodcasts)
            {
                if (!AppSettings.HasAutoSubscribed(podcastId))
                {
                    await _podcastOfflineManager.FollowPodcast(new Podcast {Id = podcastId});
                    AppSettings.SetHasAutoSubscribed(podcastId);
                }
            }
        }
    }
}
