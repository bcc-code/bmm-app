using System;
using BMM.Core.Helpers;
using BMM.Core.Implementations.Device;
using BMM.Core.Implementations.Downloading.DownloadQueue;
using BMM.Core.Implementations.Notifications;
using BMM.Core.Implementations.Security.Oidc;
using BMM.Core.Implementations.Storage;
using BMM.Core.Messages;
using BMM.Core.Models.Themes;
using BMM.UI.iOS.Implementations.Download;
using BMM.UI.iOS.Implementations.Notifications;
using Firebase.CloudMessaging;
using Foundation;
using MvvmCross;
using MvvmCross.Platforms.Ios.Core;
using MvvmCross.Plugin.Messenger;
using UIKit;
using UserNotifications;

namespace BMM.UI.iOS
{
    [Register(nameof(AppDelegate))]
    public class AppDelegate : MvxApplicationDelegate<IosSetup, Core.App>
    {
        private FirebaseMessagingDelegate _messagingDelegate;
        private bool DarkModeSupported => UIDevice.CurrentDevice.CheckSystemVersion(13, 0);

        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            var result = base.FinishedLaunching(app, options);

            // Code to start the Xamarin Test Cloud Agent
#if ENABLE_TEST_CLOUD
            Xamarin.Calabash.Start();
#endif

#if !ENABLE_TEST_CLOUD
            RegisterForNotifications(app);
#endif

            app.ApplicationIconBadgeNumber = 0;

            SetThemeForApp();
            return result;
        }

        private void SetThemeForApp()
        {
            var theme = AppSettings.SelectedTheme;

            if (theme == Theme.System)
                return;
            
            var userInterfaceStyle = theme switch
            {
                Theme.Light => UIUserInterfaceStyle.Light,
                Theme.Dark => UIUserInterfaceStyle.Dark,
                _ => UIUserInterfaceStyle.Unspecified
            };

            if (!DarkModeSupported || Window == null)
                return;

            Window.OverrideUserInterfaceStyle = userInterfaceStyle;

            if (theme == Theme.Dark)
                UIApplication.SharedApplication.SetStatusBarStyle(UIStatusBarStyle.LightContent, true);
            else
                UIApplication.SharedApplication.SetStatusBarStyle(UIStatusBarStyle.DarkContent, true);
        }

        [Export("application:continueUserActivity:restorationHandler:")]
        public override bool ContinueUserActivity(UIApplication application, NSUserActivity userActivity, UIApplicationRestorationHandler completionHandler)
        {
            var uri = new Uri(userActivity.WebPageUrl.AbsoluteString);
            return Mvx.IoCProvider.Resolve<IDeepLinkHandler>().OpenFromOutsideOfApp(uri);
        }

        public override void FailedToRegisterForRemoteNotifications(UIApplication application, NSError error)
        {
            Console.Error.WriteLine("Failed to register for remote notifications");
        }

        public override void WillTerminate(UIApplication application)
        {
            Mvx.IoCProvider?.Resolve<IDownloadQueue>()?.AppWasKilled();
        }

        /**
         * Save the completion-handler we get when the app starts back from the background.
         * This method informs iOS that the app has finished all internal processing and can fall asleep again.
         */
        public override void HandleEventsForBackgroundUrl(UIApplication application, string sessionIdentifier, Action completionHandler)
        {
            IosFileDownloader.BackgroundSessionCompletionHandler = completionHandler;
        }

        private void RegisterForNotifications(UIApplication app)
        {
            _messagingDelegate = new FirebaseMessagingDelegate();
            Messaging.SharedInstance.Delegate = _messagingDelegate;

            const UNAuthorizationOptions authOptions = UNAuthorizationOptions.Alert | UNAuthorizationOptions.Badge | UNAuthorizationOptions.Sound;
            UNUserNotificationCenter.Current.RequestAuthorization(authOptions, (granted, error) => { Console.WriteLine(granted); });

            UNUserNotificationCenter.Current.Delegate = Mvx.IoCProvider.Create<UserNotificationCenterDelegate>();

            app.RegisterForRemoteNotifications();
        }

        /// <summary>
        /// This method is called for arriving silent notifications.
        /// Regular notification are handled within the UNUserNotificationCenterDelegate.
        /// https://developer.apple.com/documentation/usernotifications/setting_up_a_remote_notification_server/pushing_background_updates_to_your_app
        /// </summary>
        public override void DidReceiveRemoteNotification(UIApplication application, NSDictionary userInfo, Action<UIBackgroundFetchResult> completionHandler)
        {
            var handler = Mvx.IoCProvider.Resolve<INotificationHandler>();

            var iosNotification = new IosNotification(userInfo);

            if (NotificationClickedWhenAppWasClosedOrInBackground(application))
                handler.UserClickedNotification(iosNotification);
            else
                handler.OnNotificationReceived(iosNotification);

            completionHandler(UIBackgroundFetchResult.NewData);
        }

        private bool NotificationClickedWhenAppWasClosedOrInBackground(UIApplication application)
            => application.ApplicationState == UIApplicationState.Inactive;

        // Just used by the UI tests
        [Export("ClearAllLocalData")]
        public void ClearAllLocalData()
        {
            Mvx.IoCProvider.Resolve<IMvxMessenger>().Publish(new LoggedOutMessage(this));
        }

        // Just used for the login in the UI tests
        public override bool OpenUrl(UIApplication app, NSUrl url, NSDictionary options)
        {
            OidcCallbackMediator.Instance.Send(url.AbsoluteString);
            return true;
        }
    }
}
