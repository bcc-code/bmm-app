using Android;
using Android.App;
using Android.Content;
using Android.OS;
using BMM.Core.Helpers;
using BMM.Core.Implementations.Notifications.Data;
using BMM.UI.Droid.Application.Actions.Interfaces;
using BMM.UI.Droid.Application.Constants;
using MvvmCross;
using MvvmCross.Platforms.Android.Views;

namespace BMM.UI.Droid
{
    [Activity(Label = "BMM",
        MainLauncher = true,
        Icon = "@drawable/app_icon",
        Theme = "@style/AppTheme.Splash",
        NoHistory = true,
        Name = "bmm.ui.droid.SplashScreenActivity",
        Exported = true)]
    [IntentFilter(new[]
        {
            PodcastNotification.Type,
            GeneralNotification.Type,
            WordOfFaithNotification.Type
        },
        Categories = new[]
        {
            Intent.CategoryDefault
        })]
    [IntentFilter(
        new[] { Intent.ActionView },
        Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable },
        AutoVerify = true,
        DataSchemes = new[] { "https", "http" },
        DataHosts = new[] { GlobalConstants.BmmUrlProd, GlobalConstants.BmmUrlInt },
        DataPathPatterns = new[]
        {
            "/archive",
            "/album/.*",
            "/track/.*",
            "/playlist/curated/.*",
            "/playlist/private/.*",
            "/playlist/shared/.*",
            "/playlist/contributor/.*",
            "/playlist/podcast/.*",
            "/podcasts/.*",
            "/playlist/latest",
            "/copyright",
            "/",
            "/daily-fra-kaare",
            "/music",
            "/speeches",
            "/contributors",
            "/featured",
            "/browse/.*"
        }
    )]
    public class SplashScreenActivity : MvxSplashScreenActivity
    {
        public static Intent UnhandledNotification;
        public static string UnhandledDeepLink;

        public SplashScreenActivity() : base(Resource.Layout.splash_screen)
        {
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            AppCenterHelper.DroidRegister();
            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);

            HandleDeepLink(Intent);
            SetNotificationToHandle();
        }
        
        private static void HandleDeepLink(Intent intent)
        {
            string deepLink = intent?.Data?.ToString();
            
            if (string.IsNullOrEmpty(deepLink))
                return;
            
            if (!Mvx.IoCProvider.TryResolve<IHandleDeepLinkAction>(out var handleDeepLinkAction))
            {
                UnhandledDeepLink = deepLink;
                return;
            }

            handleDeepLinkAction.ExecuteGuarded(deepLink);
        }

        private void SetNotificationToHandle()
        {
            if (Intent == null)
                return;

            var extras = Intent.Extras;
            var keySet = extras?.KeySet();

            if (keySet is null)
                return;

            bool isNotification = keySet.Contains(NotificationKeys.GoogleMessageId);

            if (isNotification)
                UnhandledNotification = Intent;
        }
    }
}