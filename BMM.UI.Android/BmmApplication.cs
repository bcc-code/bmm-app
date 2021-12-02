using System;
using Android.App;
using Android.Runtime;
using AndroidX.AppCompat.App;
using BMM.Core;
using BMM.Core.Implementations.Storage;
using BMM.Core.ViewModels;
using BMM.UI.Droid.Application.Activities;
using BMM.UI.Droid.Application.Fragments;
using BMM.UI.Droid.Utils;
using Java.Interop;
using MvvmCross;
using MvvmCross.Platforms.Android;
using MvvmCross.Platforms.Android.Views;

namespace BMM.UI.Droid
{
#if DEBUG
    [Application(Debuggable = true)]
#else
    [Application(Debuggable = false)]
#endif
    public class BmmApplication : MvxAndroidApplication<AndroidSetup, App>
    {
        public static bool RunsUiTest;

        public BmmApplication(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        { }

        public override void OnCreate()
        {
            base.OnCreate();
            SetThemeIfNeeded();
        }

        private void SetThemeIfNeeded()
        {
            var theme = AppSettings.SelectedTheme;

            if (theme == Core.Models.Themes.Theme.System)
                return;

            AppCompatDelegate.DefaultNightMode = ThemeUtils.GetUIModeForTheme(theme);
        }

        [Export("AudioIsPlaying")]
        public string AudioIsPlaying(string miniPlayer)
        {
            var currentActivity = Mvx.IoCProvider.Resolve<IMvxAndroidCurrentTopActivity>().Activity as MainActivity;

            if (currentActivity == null)
            {
                return "cannot find current activity";
            }

            var fragmentTag = "mini".Equals(miniPlayer, StringComparison.CurrentCultureIgnoreCase)
                ? typeof(MiniPlayerViewModel).FullName
                : typeof(PlayerViewModel).FullName;

            var fragment = currentActivity.SupportFragmentManager.FindFragmentByTag(fragmentTag);

            if (fragment == null)
            {
                return "fragment not found";
            }

            if (fragment is MiniPlayerFragment miniPlayerFragment)
            {
                return miniPlayerFragment.ViewModel.IsPlaying.ToString();
            }

            if (fragment is PlayerFragment playerFragment)
            {
                return playerFragment.ViewModel.IsPlaying.ToString();
            }

            return "invalid fragment";
        }

        [Export("InvokeUiTestBrowser")]
        public void InvokeUiTestBrowser()
        {
            RunsUiTest = true;
        }
    }
}