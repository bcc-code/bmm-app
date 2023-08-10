using Acr.UserDialogs;
using BMM.Core.Implementations.Localization.Interfaces;
using BMM.Core.Implementations.Notifications;
using BMM.Core.Translation;
using Microsoft.Maui.ApplicationModel;
using MvvmCross.Base;
using UserNotifications;

namespace BMM.UI.iOS.Implementations.Notifications
{
    public class iOSNotificationPermissionService : INotificationPermissionService
    {
        private readonly IMvxMainThreadAsyncDispatcher _mvxMainThreadAsyncDispatcher;
        private readonly IBMMLanguageBinder _bmmLanguageBinder;
        private readonly IUserDialogs _userDialogs;

        public iOSNotificationPermissionService(
            IMvxMainThreadAsyncDispatcher mvxMainThreadAsyncDispatcher,
            IBMMLanguageBinder bmmLanguageBinder,
            IUserDialogs userDialogs)
        {
            _mvxMainThreadAsyncDispatcher = mvxMainThreadAsyncDispatcher;
            _bmmLanguageBinder = bmmLanguageBinder;
            _userDialogs = userDialogs;
        }
        
        public async Task<bool> CheckIsNotificationPermissionGranted()
        {
            var status = await GetNotificationPermissionStatus();
            return status == UNAuthorizationStatus.Authorized;
        }

        /// <summary>
        /// We show notification permission request in AppDelegate, so we can't show it again here.
        /// Therefore we only can navigate to app settings. 
        /// </summary>
        public async Task RequestNotificationPermission(bool fallbackToSettings)
        {
            bool notificationsGranted = await CheckIsNotificationPermissionGranted();
            
            if (!notificationsGranted && fallbackToSettings)
                await ShowGoToSettingsMessage();
        }
        
        private async Task ShowGoToSettingsMessage()
        {
            var confirmationMessageConfig = new ConfirmConfig
            {
                Title = _bmmLanguageBinder[Translations.SettingsViewModel_OptionEnablePushHeader],
                Message = _bmmLanguageBinder[Translations.SettingsViewModel_PushNotificationsGoToSettings],
                OkText = _bmmLanguageBinder[Translations.Global_GoToSettings],
                CancelText = _bmmLanguageBinder[Translations.Global_NotNow]
            };

            bool result = await _userDialogs.ConfirmAsync(confirmationMessageConfig);

            if (result)
            {
                await _mvxMainThreadAsyncDispatcher.ExecuteOnMainThreadAsync(async () =>
                {
                    AppInfo.ShowSettingsUI();
                });
            }
        }
        
        private async Task<UNAuthorizationStatus> GetNotificationPermissionStatus()
        {
            UNAuthorizationStatus authorizationStatus = UNAuthorizationStatus.NotDetermined;

            await _mvxMainThreadAsyncDispatcher.ExecuteOnMainThreadAsync(async () =>
            {
                var notificationSettings = await UNUserNotificationCenter.Current.GetNotificationSettingsAsync();
                authorizationStatus = notificationSettings.AuthorizationStatus;
            });

            return authorizationStatus;
        }
    }
}