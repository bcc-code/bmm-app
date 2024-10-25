using BMM.Api.Implementation.Models;
using BMM.Core.Implementations.Connection;
using BMM.Core.Implementations.FirebaseRemoteConfig;
using BMM.Core.Implementations.Notifications;
using BMM.Core.Implementations.Storage;
using BMM.Core.Messages;
using MvvmCross.Plugin.Messenger;

namespace BMM.Core.Implementations.Badge;

public class BadgeService : IBadgeService
{
    private readonly ISettingsStorage _settingsStorage;
    private readonly IFirebaseRemoteConfig _firebaseRemoteConfig;
    private readonly INotificationDisplayer _notificationDisplayer;
    private bool _isBadgeSet;

    public BadgeService(
        ISettingsStorage settingsStorage,
        IFirebaseRemoteConfig firebaseRemoteConfig,
        INotificationDisplayer notificationDisplayer)
    {
        _settingsStorage = settingsStorage;
        _firebaseRemoteConfig = firebaseRemoteConfig;
        _notificationDisplayer = notificationDisplayer;
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

    public async Task Set()
    {
        AppSettings.IsBadgeSet = IsBadgeSet = true;
        AppSettings.BadgeSetAt = DateTime.UtcNow;
    }

    public async Task Remove()
    {
        AppSettings.IsBadgeSet = IsBadgeSet = false;
        AppSettings.BadgeSetAt = DateTime.MinValue;
    }

    public async Task VerifyBadge()
    {
        IsBadgeSet = AppSettings.IsBadgeSet;

        if (AppSettings.BadgeSetAt.AddDays(1) < DateTime.UtcNow)
            await Remove();
    }

    public event EventHandler BadgeChanged;
}