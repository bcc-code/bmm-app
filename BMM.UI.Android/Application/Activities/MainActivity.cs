﻿using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using BMM.Core.Helpers.PresentationHints;
using BMM.Core.NewMediaPlayer.Abstractions;
using BMM.Core.ViewModels;
using BMM.Core.ViewModels.Base;
using BMM.UI.Droid.Application.Actions.Interfaces;
using BMM.UI.Droid.Application.Fragments;
using BMM.UI.Droid.Application.Helpers;
using BMM.UI.Droid.Application.NewMediaPlayer.Controller;
using Google.Android.Material.BottomNavigation;
using MvvmCross.ViewModels;
using MvvmCross;
using MvvmCross.Platforms.Android.Presenters;
using MvvmCross.Platforms.Android.Views.Fragments;

namespace BMM.UI.Droid.Application.Activities
{
    [Activity(
         Label = "BMM",
         Theme = "@style/AppTheme",
         LaunchMode = LaunchMode.SingleTask,
         Name = "bmm.ui.droid.application.activities.MainActivity",
         WindowSoftInputMode = SoftInput.AdjustResize | SoftInput.AdjustPan,
         ScreenOrientation = ScreenOrientation.Portrait,
         Exported = true
    )]
    public class MainActivity : BaseFragmentActivity<MainActivityViewModel>
    {
        private IMediaPlayer _mediaPlayer;
        private AndroidMediaPlayer _androidPlayer;
        private BottomNavigationView? _bottomNavigationView;
        private FrameLayout _miniPlayerFrame;
        private static int? _currentAppTheme;

        private BottomNavigationView BottomNavigationView
            => _bottomNavigationView ??= FindViewById<BottomNavigationView>(Resource.Id.bottom_navigation);

        private FrameLayout MiniPlayerFrame
            => _miniPlayerFrame ??= FindViewById<FrameLayout>(Resource.Id.miniplayer_frame);

        private PlayerFragment PlayerFragment => SupportFragmentManager.FindFragmentById(Resource.Id.player_frame) as PlayerFragment;

        protected override void OnCreate(Bundle bundle)
        {
            // We see a lot of crashes in this method and the theory is that the app is opened immediately here skipping the SplashScreen.
            // And then it would not be initialized and probably crash right away.
            SetupHelper.EnsureInitialized();

            _androidPlayer = Mvx.IoCProvider.Resolve<IPlatformSpecificMediaPlayer>() as AndroidMediaPlayer;
            _mediaPlayer = Mvx.IoCProvider.Resolve<IMediaPlayer>();
            _mediaPlayer.ViewHasBeenDestroyed(ThemeHasChanged());

            SetContentView(Resource.Layout.activity_main);
            base.OnCreate(bundle);

            Xamarin.Essentials.Platform.Init(this, bundle);
            var viewPresenter = Mvx.IoCProvider.GetSingleton<IMvxAndroidViewPresenter>();
            
            if (bundle == null)
            {
                var menuViewModelRequest = new MvxViewModelRequest(typeof(MenuViewModel), null, null);
                viewPresenter.Show(menuViewModelRequest);
            }

            viewPresenter.AddPresentationHintHandler<ClearAllNavBackStackHint>(ClearAllNavBackStackHintHandler);
            viewPresenter.AddPresentationHintHandler<MenuClickedHint>(NavigationRootChangedHintHandler);

            SetCurrentTheme();
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public async Task<bool> ClearAllNavBackStackHintHandler(ClearAllNavBackStackHint hint)
        {
            if (!SupportFragmentManager.IsDestroyed && !SupportFragmentManager.IsStateSaved)
                SupportFragmentManager.PopBackStackImmediate();

            _mediaPlayer.ViewHasBeenDestroyed(false);
            return true;
        }

        public async Task<bool> NavigationRootChangedHintHandler(MenuClickedHint hint)
        {
            ClearBackStack();
            return true;
        }

        public void SetBottomBarVisibility(ViewStates viewState)
        {
            var bottomBar = BottomNavigationView;

            if (bottomBar != null && bottomBar.Visibility != viewState)
                bottomBar.Visibility = viewState;
        }

        public int GetSnackbarBottomMargin()
        {
            int bottomBarHeight = BottomNavigationView?.Height ?? 0;
            int miniPlayerHeight = 0;

            var playerFragment = PlayerFragment;

            if (playerFragment != null && !playerFragment.IsOpen)
                miniPlayerHeight = MiniPlayerFrame?.Height ?? 0;

            return bottomBarHeight + miniPlayerHeight;
        }

        private void ClearBackStack()
        {
            for (var i = 0; i < SupportFragmentManager.BackStackEntryCount; i++)
            {
                SupportFragmentManager.PopBackStack();
            }
        }

        public override void OnBackPressed()
        {
            var playerFrag = PlayerFragment;

            if (playerFrag != null && playerFrag.IsOpen)
            {
                if (playerFrag.IsVisible)
                {
                    playerFrag.ClosePlayer();
                    return;
                }
            }
        
            base.OnBackPressed();
        }

        protected override void OnResume()
        {
            base.OnResume();

            var docVms = SupportFragmentManager
                .Fragments
                .OfType<MvxFragment>()
                .Select(x => x.ViewModel)
                .OfType<DocumentsViewModel>();

            foreach (var documentsViewModel in docVms)
            {
                documentsViewModel.RefreshInBackground();
            }
        }

        private static void SetCurrentTheme()
        {
            _currentAppTheme = AppCompatDelegate.DefaultNightMode;
        }

        private static bool ThemeHasChanged()
        {
            if (_currentAppTheme == null)
                return false;

            return _currentAppTheme != AppCompatDelegate.DefaultNightMode;
        }

        protected override void OnStart()
        {
            base.OnStart();

            _androidPlayer.AfterConnectedAction = async () =>
            {
                await _androidPlayer.RestoreLastPlayingTrackAfterThemeChangedIfAvailable();
                await HandleNotification();
                await HandleDeepLink();
            };

            _androidPlayer.Connect(this);
        }
        
        private static async Task HandleNotification()
        {
            await Mvx.IoCProvider.Resolve<IHandleNotificationAction>().ExecuteGuarded();
        }

        private static async Task HandleDeepLink()
        {
            await Mvx.IoCProvider.Resolve<IHandleDeepLinkAction>().ExecuteGuarded(SplashScreenActivity.UnhandledDeepLink);
        }

        protected override void OnDestroy()
        {
            if (ThemeHasChanged())
                _androidPlayer.SaveCurrentTrackAndQueueAfterThemeChanged();

            _androidPlayer.Disconnect();
            base.OnDestroy();
        }
    }
}