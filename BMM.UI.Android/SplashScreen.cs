using Android;
using Android.App;
using Android.Content;
using Android.OS;
using BMM.Core.Helpers;
using BMM.Core.Implementations.Notifications.Data;
using BMM.UI.Droid.Application.Constants;
using MvvmCross.Platforms.Android.Views;

namespace BMM.UI.Droid
{
    [Activity(Label = "BMM",
        MainLauncher = true,
        Icon = "@drawable/app_icon",
        Theme = "@style/AppTheme.Splash",
        NoHistory = true,
        Name = "bmm.ui.droid.SplashScreen",
        Exported = true)]
    [IntentFilter(new[] {PodcastNotification.Type, GeneralNotification.Type, WordOfFaithNotification.Type}, Categories = new[] {Intent.CategoryDefault})]
    public class SplashScreen : MvxSplashScreenActivity
    {
        public static Intent UnhandledIntent;

        public SplashScreen() : base(Resource.Layout.splash_screen)
        {
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            AppCenterHelper.DroidRegister();
            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);

            CheckNotification(savedInstanceState);

            /* We use it in order to prevent SplashScreen from freezing when we tap on notification.
             * Problem is described in the link: https://github.com/MvvmCross/MvvmCross/issues/3042#issuecomment-454200794
             * Issue doesn't exist in MvvmCross version 6.3.+ so if we update it than check should be deleted from the project. */
            if (!IsTaskRoot)
            {
                Finish();
            }
        }

        private void CheckNotification(Bundle savedInstanceState)
        {
            if (savedInstanceState != null || Intent == null)
                return;

            var extras = Intent.Extras;
            var keySet = extras?.KeySet();

            if (keySet is null)
                return;

            bool isNotification = keySet.Contains(NotificationKeys.GoogleMessageId);

            if (isNotification)
                UnhandledIntent = Intent;
        }
    }
}