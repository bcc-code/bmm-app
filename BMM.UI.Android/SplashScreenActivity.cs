using System;
using Android.App;
using Android.Content;
using Android.OS;
using BMM.Core.Helpers;
using BMM.Core.Implementations.Notifications.Data;
using BMM.UI.Droid.Application.Constants;
using MvvmCross.Exceptions;
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
            GeneralNotification.Type
        },
        Categories = new[]
        {
            Intent.CategoryDefault
        })]
    public class SplashScreenActivity : MvxSplashScreenActivity
    {
        public static Intent UnhandledNotification;

        public SplashScreenActivity() : base(Resource.Layout.splash_screen)
        {
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            AppCenterHelper.DroidRegister();
            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);

            SetNotificationToHandle();
        }
        
        /// <summary>
        /// Prevents MvvmCross crash, when calling CancelMonitor in base.OnPause().
        /// </summary>
        protected override void OnPause()
        {
            try
            {
                base.OnPause();
            }
            catch (MvxException e)
            {
                Console.WriteLine(e);
            }
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