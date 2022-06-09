using System.Threading.Tasks;
using BMM.Core.Implementations.Analytics;
using BMM.Core.Implementations.Caching;
using BMM.Core.Implementations.Podcasts;
using BMM.Core.Implementations.Security.Oidc;
using BMM.Core.Implementations.Security.Oidc.Interfaces;
using BMM.Core.Implementations.TrackCollections;

namespace BMM.Core.Implementations.Security
{
    public class LogoutService : ILogoutService
    {
        private readonly IOidcAuthService _oidcAuthService;
        private readonly IAnalytics _analytics;
        private readonly ICache _cache;
        private readonly IOfflineTrackCollectionStorage _trackCollectionStorage;
        private readonly IPodcastOfflineManager _podcastOfflineManager;

        public LogoutService(
            IOidcAuthService oidcAuthService,
            IAnalytics analytics,
            ICache cache,
            IOfflineTrackCollectionStorage trackCollectionStorage,
            IPodcastOfflineManager podcastOfflineManager
        )
        {
            _oidcAuthService = oidcAuthService;
            _analytics = analytics;
            _cache = cache;
            _trackCollectionStorage = trackCollectionStorage;
            _podcastOfflineManager = podcastOfflineManager;
        }

        public async Task PerformLogout()
        {
            _analytics.LogEvent("Logout current user");

            await _oidcAuthService.PerformLogout();

            await _cache.Clear();
            await _trackCollectionStorage.Clear();
            await _podcastOfflineManager.Clear();
        }
    }
}