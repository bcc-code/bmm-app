using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using BMM.Core.Helpers;
using BMM.Core.Helpers.PresentationHints;
using BMM.Core.Implementations.Notifications;
using BMM.Core.NewMediaPlayer.Abstractions;
using BMM.Core.ViewModels;
using BMM.Core.ViewModels.Base;
using BMM.UI.Droid.Application.Fragments;
using BMM.UI.Droid.Application.Helpers;
using BMM.UI.Droid.Application.Implementations.Notifications;
using BMM.UI.Droid.Application.NewMediaPlayer.Controller;
using Google.Android.Material.BottomNavigation;
using MvvmCross.ViewModels;
using MvvmCross;
using MvvmCross.Navigation;
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
    [IntentFilter(
        new[] { Intent.ActionView },
        Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable },
        AutoVerify = true,
        DataSchemes = new[] { "https", "http" },
        DataHosts = new[] { GlobalConstants.BmmUrlProd, GlobalConstants.BmmUrlInt },
        DataPathPatterns = new[]
        {
            "/archive", "/album/.*", "/track/.*", "/playlist/curated/.*", "/playlist/private/.*", "/playlist/shared/.*", "/playlist/contributor/.*", "/playlist/podcast/.*", "/podcasts/.*", "/playlist/latest", "/copyright", "/", "/daily-fra-kaare", "/music", "/speeches", "/contributors", "/featured", "/browse/.*"
        }
    )]
    public class MainActivity : BaseFragmentActivity<MainActivityViewModel>
    {
        private IMediaPlayer _mediaPlayer;
        private AndroidMediaPlayer _androidPlayer;
        private BottomNavigationView? _bottomNavigationView;

        private string _unhandledDeepLink;

        protected override void OnCreate(Bundle bundle)
        {
            // We see a lot of crashes in this method and the theory is that the app is opened immediately here skipping the SplashScreen.
            // And then it would not be initialized and probably crash right away.
            SetupHelper.EnsureInitialized();

            _androidPlayer = Mvx.IoCProvider.Resolve<IPlatformSpecificMediaPlayer>() as AndroidMediaPlayer;
            _mediaPlayer = Mvx.IoCProvider.Resolve<IMediaPlayer>();
            _mediaPlayer.ViewHasBeenDestroyed();

            SetContentView(Resource.Layout.activity_main);
            base.OnCreate(bundle);

            Xamarin.Essentials.Platform.Init(this, bundle);

            // This is necessary when we open the app through a deep link. For some reason Start() is not called automatically.
            // ToDo: Due to the fact that the app doesn't start properly we also see a white screen instead of the SplashScreen
            var startup = Mvx.IoCProvider.Resolve<IMvxAppStart>();
            startup.Start();

            var viewPresenter = Mvx.IoCProvider.GetSingleton<IMvxAndroidViewPresenter>();

            if (Intent?.Data != null)
            {
                _unhandledDeepLink = Intent.Data.ToString();
            }

            if (bundle == null)
            {
                var menuViewModelRequest = new MvxViewModelRequest(typeof(MenuViewModel), null, null);
                viewPresenter.Show(menuViewModelRequest);
            }

            viewPresenter.AddPresentationHintHandler<ClearAllNavBackStackHint>(ClearAllNavBackStackHintHandler);
            viewPresenter.AddPresentationHintHandler<MenuClickedHint>(NavigationRootChangedHintHandler);

            // Cleaning the back stack to have a clean state. We don't need to do this when the app is started with a deeplink
            // because the this activity is in this case always called first so there is nothing to remove in the back stack.
            if (Intent?.Data == null)
                Mvx.IoCProvider.Resolve<IMvxNavigationService>().ChangePresentation(new ClearAllNavBackStackHint());
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

            _mediaPlayer.ViewHasBeenDestroyed();

            return true;
        }

        public async Task<bool> NavigationRootChangedHintHandler(MenuClickedHint hint)
        {
            ClearBackStack();
            return true;
        }

        public void SetBottomBarVisibility(ViewStates viewState)
        {
            var bottomBar = _bottomNavigationView ??= FindViewById<BottomNavigationView>(Resource.Id.bottom_navigation);

            if (bottomBar != null && bottomBar.Visibility != viewState)
                bottomBar.Visibility = viewState;
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
            var playerFrag = SupportFragmentManager.FindFragmentById(Resource.Id.player_frame) as PlayerFragment;

            if (playerFrag != null && playerFrag.IsOpen)
            {
                if (playerFrag.IsVisible)
                {
                    playerFrag.ClosePlayer();
                }
                else
                {
                    var queueFrag = SupportFragmentManager.FindFragmentById(Resource.Id.content_frame) as QueueFragment;
                    if (queueFrag != null)
                        queueFrag.CloseQueue();
                    else
                        base.OnBackPressed();
                }
            }
            else
            {
                base.OnBackPressed();
            }
        }

        protected override void OnNewIntent(Intent intent)
        {
            if (intent.Data != null)
            {
                var deepLink = new System.Uri(intent.Data.ToString());
                Mvx.IoCProvider.Resolve<IDeepLinkHandler>().OpenFromOutsideOfApp(deepLink);
            }
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

        private void CheckUnhandledIntent()
        {
            if (SplashScreen.UnhandledIntent == null)
                return;

            var intent = SplashScreen.UnhandledIntent;
            SplashScreen.UnhandledIntent = null;

            Mvx.IoCProvider.Resolve<INotificationHandler>().UserClickedNotification(new AndroidIntentNotification(intent));
        }

        protected override void OnStart()
        {
            base.OnStart();

            _androidPlayer.AfterConnectedAction = () =>
            {
                CheckUnhandledIntent();
                HandleDeepLink();
            };

            _androidPlayer.Connect(this);
        }

        private void HandleDeepLink()
        {
            if (_unhandledDeepLink == null)
                return;

            var url = new System.Uri(_unhandledDeepLink);
            Mvx.IoCProvider.Resolve<IDeepLinkHandler>().OpenFromOutsideOfApp(url);
            _unhandledDeepLink = null;
        }

        protected override void OnDestroy()
        {
            _androidPlayer.Disconnect();
            base.OnDestroy();
        }
    }
}