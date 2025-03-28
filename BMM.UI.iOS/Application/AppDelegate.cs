using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Acr.UserDialogs;
using BMM.Core.Helpers;
using BMM.Core.Implementations.Analytics;
using BMM.Core.Implementations.Device;
using BMM.Core.Implementations.Downloading.DownloadQueue;
using BMM.Core.Implementations.Languages;
using BMM.Core.Implementations.Notifications;
using BMM.Core.Implementations.Player.Interfaces;
using BMM.Core.Implementations.Security.Oidc;
using BMM.Core.Implementations.Storage;
using BMM.Core.Messages;
using BMM.Core.Models.Themes;
using BMM.UI.iOS.Actions.Interfaces;
using BMM.UI.iOS.Implementations.Download;
using BMM.UI.iOS.Implementations.Notifications;
using BMM.UI.iOS.Utils;
using Firebase.CloudMessaging;
using Foundation;
using Intents;
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
        public static UIWindow MainWindow;

        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            var result = base.FinishedLaunching(app, options);

            // Code to start the Xamarin Test Cloud Agent
#if ENABLE_TEST_CLOUD
            // https://github.com/microsoft/appcenter/issues/2416
            //            Xamarin.Calabash.Start();
#endif

// #if !ENABLE_TEST_CLOUD
            RegisterForNotifications(app);
// #endif

            app.ApplicationIconBadgeNumber = new IntPtr(0);

            SetThemeForApp();
            MainWindow = Window;
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
            string url = userActivity?.WebPageUrl?.AbsoluteString;
            
            if (string.IsNullOrEmpty(url))
                return false; 
            
            var deepLinkHandler = Mvx.IoCProvider.Resolve<IDeepLinkHandler>();
            deepLinkHandler.SetDeepLinkWillStartPlayerIfNeeded(url);
            return deepLinkHandler.OpenFromOutsideOfApp(new Uri(url));
        }

        public override void FailedToRegisterForRemoteNotifications(UIApplication application, NSError error)
        {
            Console.Error.WriteLine("Failed to register for remote notifications");
        }

        public override void WillTerminate(UIApplication application)
        {
            Mvx.IoCProvider?.Resolve<IDownloadQueue>()?.AppWasKilled();
        }
        
        public override void DidEnterBackground(UIApplication application)
        {
            base.DidEnterBackground(application);
            ApplicationStateWatcher.State = ApplicationState.Background;
        }

        public override void WillEnterForeground(UIApplication application)
        {
            base.WillEnterForeground(application);
            ApplicationStateWatcher.State = ApplicationState.Foreground;
        }

        public override async void HandleIntent(UIApplication application, INIntent intent, Action<INIntentResponse> completionHandler)
        {
            if (!(intent is INPlayMediaIntent playMediaIntent))
                return;

            var handleSiriMediaPlayRequestAction = Mvx.IoCProvider.Resolve<IHandleSiriMediaPlayRequestAction>();
            var intentResponse = await handleSiriMediaPlayRequestAction!.ExecuteGuarded(playMediaIntent);
            
            completionHandler(new INPlayMediaIntentResponse(intentResponse, null));
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

        //UI Tests
        [Export("GoToLink:")]
        public void GoToLink(string link)
        {
            Mvx.IoCProvider.Resolve<IDeepLinkHandler>().OpenFromInsideOfApp(new Uri(link));
        }
        
        // Just used for the login in the UI tests
        public override bool OpenUrl(UIApplication app, NSUrl url, NSDictionary options)
        {
            OidcCallbackMediator.Instance.Send(url.AbsoluteString);
            return true;
        }
    }
}
