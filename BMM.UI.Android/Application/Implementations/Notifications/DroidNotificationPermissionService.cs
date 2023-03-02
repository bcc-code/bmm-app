using Acr.UserDialogs;
using Android.OS;
using AndroidX.Core.App;
using BMM.Core.Implementations.Localization.Interfaces;
using BMM.Core.Implementations.Notifications;
using BMM.Core.Translation;
using BMM.UI.Droid.Application.Implementations.App;
using Microsoft.Maui.ApplicationModel;
using MvvmCross.Base;
using AndroidApp = Android.App.Application;

namespace BMM.UI.Droid.Application.Implementations.Notifications
{
    public class DroidNotificationPermissionService : INotificationPermissionService
    {
        private const int AndroidTiramisuVersionCode = 33;
        private const string PostNotificationPermissionName = "android.permission.POST_NOTIFICATIONS";
        
        private readonly IUserDialogs _bmmUserDialogs;
        private readonly IBMMLanguageBinder _languageBinder;
        private readonly ISystemSettingsService _systemSettingsService;
        private readonly IMvxMainThreadAsyncDispatcher _mvxMainThreadAsyncDispatcher;

        public DroidNotificationPermissionService(
            IUserDialogs bmmUserDialogs,
            IBMMLanguageBinder languageBinder,
            ISystemSettingsService systemSettingsService,
            IMvxMainThreadAsyncDispatcher mvxMainThreadAsyncDispatcher)
        {
            _bmmUserDialogs = bmmUserDialogs;
            _languageBinder = languageBinder;
            _systemSettingsService = systemSettingsService;
            _mvxMainThreadAsyncDispatcher = mvxMainThreadAsyncDispatcher;
        }

        public Task<bool> CheckIsNotificationPermissionGranted()
        {
            return Task.FromResult(NotificationManagerCompat.From(AndroidApp.Context).AreNotificationsEnabled());
        }

        public async Task RequestNotificationPermission(bool fallbackToSettings)
        {
            await _mvxMainThreadAsyncDispatcher.ExecuteOnMainThreadAsync(async () =>
            {
                if ((int)Build.VERSION.SdkInt < AndroidTiramisuVersionCode)
                {
                    if (fallbackToSettings)
                        await ShowGoToSettingsMessage();
                    
                    return;
                }
                
                var result = await Permissions.RequestAsync<NotificationPermission>();
                if (result == PermissionStatus.Granted || !fallbackToSettings)
                    return;

                await ShowGoToSettingsMessage();
            });
        }

        private async Task ShowGoToSettingsMessage()
        {
            var confirmationMessageConfig = new ConfirmConfig
            {
                Title = _languageBinder[Translations.SettingsViewModel_OptionEnablePushHeader],
                Message = _languageBinder[Translations.SettingsViewModel_PushNotificationsGoToSettings],
                OkText = _languageBinder[Translations.Global_GoToSettings],
                CancelText = _languageBinder[Translations.Global_NotNow]
            };

            bool result = await _bmmUserDialogs.ConfirmAsync(confirmationMessageConfig);

            if (result)
                await _systemSettingsService.GoToSettings();
        }
    }

    public class NotificationPermission : Permissions.BasePlatformPermission
    {
        private const string PostNotificationPermissionName = "android.permission.POST_NOTIFICATIONS";

        public override (string androidPermission, bool isRuntime)[] RequiredPermissions =>
            new[]
            {
                (PostNotificationPermissionName, true)
            };
    }
}