using BMM.Core.Implementations.Connection;
using BMM.Core.Implementations.FirebaseRemoteConfig;
using BMM.Core.Implementations.Storage;
using BMM.Core.NewMediaPlayer.Abstractions;

namespace BMM.Core.Implementations.Badge;

public class BadgeService : IBadgeService
{
    private readonly ISettingsStorage _settingsStorage;
    private bool _isBadgeSet;

    public BadgeService(
        ISettingsStorage settingsStorage)
    {
        _settingsStorage = settingsStorage;
    }

    public bool IsBadgeSet
    {
        get => _isBadgeSet;
        private set
        {
            _isBadgeSet = value;
            BadgeChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public async Task<bool> SetIfPossible(int trackId)
    {
        if (!await _settingsStorage.GetBibleStudyBadgeEnabled())
            return false;

        if (!await _settingsStorage.GetRemoveBadgeOnStreakPointOnlyEnabled()
            && AppSettings.LastPlayedCurrentPodcastTrackId == trackId)
        {
            Remove();
            return false;
        }
        
        AppSettings.IsBadgeSet = IsBadgeSet = true;
        AppSettings.BadgeSetAt = DateTime.UtcNow;
        return true;
    }
    
    public void Remove()
    {
        AppSettings.IsBadgeSet = IsBadgeSet = false;
        AppSettings.BadgeSetAt = DateTime.MinValue;
    }

    public async Task VerifyBadge()
    {
        IsBadgeSet = AppSettings.IsBadgeSet;

        if (AppSettings.BadgeSetAt.AddDays(1) < DateTime.UtcNow)
            Remove();
    }

    public event EventHandler BadgeChanged;
}