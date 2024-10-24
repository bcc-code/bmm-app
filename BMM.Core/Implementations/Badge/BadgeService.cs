using BMM.Api.Implementation.Models;
using BMM.Core.Implementations.Connection;
using BMM.Core.Implementations.FirebaseRemoteConfig;

namespace BMM.Core.Implementations.Badge;

public class BadgeService : IBadgeService
{
    private readonly ISettingsStorage _settingsStorage;
    private readonly IFirebaseRemoteConfig _firebaseRemoteConfig;

    public BadgeService(
        ISettingsStorage settingsStorage,
        IFirebaseRemoteConfig firebaseRemoteConfig)
    {
        _settingsStorage = settingsStorage;
        _firebaseRemoteConfig = firebaseRemoteConfig;
    }
    
    public async Task<bool> ShouldShowBadgeFor(Track track, int? podcastId)
    {
        return !track.HasListened
               && await _settingsStorage.GetBibleStudyBadgeEnabled()
               && _firebaseRemoteConfig.CurrentPodcastId == podcastId;
    }
}