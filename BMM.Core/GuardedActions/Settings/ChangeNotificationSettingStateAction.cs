using System.Threading.Tasks;
using BMM.Core.GuardedActions.Base;
using BMM.Core.GuardedActions.Settings.Interfaces;
using BMM.Core.Implementations.Connection;
using BMM.Core.Implementations.Notifications;

namespace BMM.Core.GuardedActions.Settings
{
    public class ChangeNotificationSettingStateAction : GuardedActionWithParameter<bool>, IChangeNotificationSettingStateAction
    {
        private readonly INotificationPermissionService _notificationPermissionService;
        private readonly ISettingsStorage _settingsStorage;

        public ChangeNotificationSettingStateAction(
            INotificationPermissionService notificationPermissionService,
            ISettingsStorage settingsStorage)
        {
            _notificationPermissionService = notificationPermissionService;
            _settingsStorage = settingsStorage;
        }
        
        protected override async Task Execute(bool shouldEnable)
        {
            bool notificationPermissionGranted = await _notificationPermissionService.CheckIsNotificationPermissionGranted();

            if (!notificationPermissionGranted && !shouldEnable)
            {
                // in this case push notifications are disabled in the device settings, so no need to also disable it in the application
                // as the user may be mislead when he/she changes the permission in the settings and the notification
                // is still disabled in the app
                return;
            }

            await _settingsStorage.SetPushNotificationsAllowed(shouldEnable);

            if (!notificationPermissionGranted && shouldEnable)
                await _notificationPermissionService.RequestNotificationPermission(true);
        }
    }
}