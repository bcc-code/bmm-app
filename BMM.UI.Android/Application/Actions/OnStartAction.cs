using System.Threading.Tasks;
using Acr.UserDialogs;
using Android.App;
using Android.Content;
using Android.Net;
using Android.Provider;
using BMM.Api.Implementation.Clients.Contracts;
using BMM.Core.GuardedActions.App.Interfaces;
using BMM.Core.GuardedActions.Base;
using BMM.Core.Implementations.Analytics;
using BMM.Core.Implementations.Localization;
using BMM.Core.Implementations.Localization.Interfaces;
using BMM.Core.Implementations.Notifications;
using BMM.Core.Translation;
using BMM.UI.Droid.Application.Helpers;
using MvvmCross.Platforms.Android;
using Uri = Android.Net.Uri;

namespace BMM.UI.Droid.Application.Actions
{
    public class OnStartAction : GuardedAction, IOnStartAction
    {
        private readonly IMvxAndroidCurrentTopActivity _mvxAndroidCurrentTopActivity;
        private readonly IUserDialogs _userDialogs;
        private readonly ISdkVersionHelper _sdkVersionHelper;
        private readonly IAnalytics _analytics;
        private readonly INotificationPermissionService _notificationPermissionService;

        private IBMMLanguageBinder LanguageBinder => BMMLanguageBinderLocator.TextSource;

        public OnStartAction(
            IMvxAndroidCurrentTopActivity mvxAndroidCurrentTopActivity,
            IUserDialogs userDialogs,
            ISdkVersionHelper sdkVersionHelper,
            IAnalytics analytics,
            INotificationPermissionService notificationPermissionService)
        {
            _mvxAndroidCurrentTopActivity = mvxAndroidCurrentTopActivity;
            _userDialogs = userDialogs;
            _sdkVersionHelper = sdkVersionHelper;
            _analytics = analytics;
            _notificationPermissionService = notificationPermissionService;
        }
        
        protected override async Task Execute()
        {
            await SetNotificationPermission();
            ShowBackgroundActivityRestrictedMessage();
        }

        private async Task SetNotificationPermission()
        {
            if (await _notificationPermissionService.CheckIsNotificationPermissionGranted())
                return;

            await _notificationPermissionService.RequestNotificationPermission(false);
        }

        private void ShowBackgroundActivityRestrictedMessage()
        {
            if (!_sdkVersionHelper.SupportsBackgroundActivityRestriction)
                return;

            var activityManager = (ActivityManager)_mvxAndroidCurrentTopActivity.Activity.GetSystemService(Context.ActivityService);
            bool backgroundRestricted = activityManager!.IsBackgroundRestricted;

            if (!backgroundRestricted)
                return;

            var alertConfig = new PromptConfig
            {
                Title = LanguageBinder[Translations.Global_BackgroundActivityRestrictedTitle],
                Message = LanguageBinder[Translations.Global_BackgroundActivityRestrictedMessage],
                CancelText = LanguageBinder[Translations.Global_NotNow],
                OkText = LanguageBinder[Translations.Global_GoToSettings],
                OnAction = OnAction
            };

            _userDialogs.Prompt(alertConfig);
            _analytics.LogEvent(Event.BackgroundActivityRestrictedPopupShown);
        }

        private void OnAction(PromptResult result)
        {
            if (!result.Ok)
                return;
            
            var intent = new Intent(
                Settings.ActionApplicationDetailsSettings,
                Uri.FromParts("package", _mvxAndroidCurrentTopActivity.Activity.PackageName, null));
                
            _mvxAndroidCurrentTopActivity.Activity.StartActivity(intent);
        }
    }
}