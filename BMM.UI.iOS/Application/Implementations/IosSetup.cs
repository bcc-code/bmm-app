using System.Net.Http;
using Acr.UserDialogs;
using BMM.Api.Abstraction;
using BMM.Api.Framework;
using BMM.Api.Framework.HTTP;
using BMM.Core;
using BMM.Core.Helpers;
using BMM.Core.Implementations;
using BMM.Core.Implementations.Analytics;
using BMM.Core.Implementations.Downloading.FileDownloader;
using BMM.Core.Implementations.DownloadManager;
using BMM.Core.Implementations.Exceptions;
using BMM.Core.Implementations.FileStorage;
using BMM.Core.Implementations.FirebaseRemoteConfig;
using BMM.Core.Implementations.Notifications;
using BMM.Core.Implementations.Security;
using BMM.Core.Implementations.UI;
using BMM.Core.NewMediaPlayer;
using BMM.Core.NewMediaPlayer.Abstractions;
using BMM.UI.iOS.DownloadManager;
using BMM.UI.iOS.Helpers;
using BMM.UI.iOS.Implementations;
using BMM.UI.iOS.Implementations.Download;
using BMM.UI.iOS.Implementations.Notifications;
using BMM.UI.iOS.Networking;
using BMM.UI.iOS.NewMediaPlayer;
using BMM.UI.iOS.UI;
using FFImageLoading;
using IdentityModel.OidcClient.Browser;
using MvvmCross;
using MvvmCross.Base;
using MvvmCross.IoC;
using MvvmCross.Platforms.Ios.Core;
using MvvmCross.Platforms.Ios.Presenters;
using MvvmCross.ViewModels;

namespace BMM.UI.iOS
{
    public class IosSetup : MvxIosSetup<App>
    {
        protected override void InitializeFirstChance()
        {
            base.InitializeFirstChance();

            // Since we need Firebase already in the dependency injection initialization we have to configure it here.
            // FinishedLaunching of the AppDelegate (as suggested in the Firebase docs) would be to late
            // because it's called after creating the DI containers in Mvx.
            Firebase.Core.App.Configure();

            Mvx.IoCProvider.LazyConstructAndRegisterSingleton<IStopwatchManager, StopwatchManager>();
            var stopwatch = Mvx.IoCProvider.Resolve<IStopwatchManager>();
            stopwatch.StartAndGetStopwatch(StopwatchType.AppStart);

            Mvx.IoCProvider.RegisterType<INotificationSubscriptionTokenProvider, FirebaseTokenProvider>();

#if DEBUG
            Mvx.IoCProvider.RegisterType<ILogger>(
                // Use direct instance of UserDialog.Instance to avoid nested-loops
                () => new ErrorDialogDisplayingLogger(UserDialogs.Instance, new IosLogger(), Mvx.IoCProvider.Resolve<IMvxMainThreadAsyncDispatcher>())
            );
#else
            Mvx.IoCProvider.RegisterType<ILogger, IosLogger>();
#endif

            Mvx.IoCProvider.ConstructAndRegisterSingleton<IUiDependentExecutor, BottomNavigationLoadedDependentExecutor>();
            Mvx.IoCProvider.RegisterType<ISimpleHttpClient, NetworkAccessAwareHttpClient>();
            Mvx.IoCProvider.LazyConstructAndRegisterSingleton<ITrackMediaHelper, TrackMediaHelper>();
            Mvx.IoCProvider.LazyConstructAndRegisterSingleton<IStorageManager, StorageManager>();
            Mvx.IoCProvider.LazyConstructAndRegisterSingleton<INotificationDisplayer, NotificationDisplayer>();
            Mvx.IoCProvider.RegisterType<UserNotificationCenterDelegate>();
            Mvx.IoCProvider.RegisterType<IClipboardService, ClipboardService>();
            Mvx.IoCProvider.LazyConstructAndRegisterSingleton<IPlatformSpecificRemoteConfig, IosFirebaseRemoteConfig>();
            Mvx.IoCProvider.LazyConstructAndRegisterSingleton<IDeviceSupportVersionChecker, IosSupportVersionChecker>();

            Mvx.IoCProvider.RegisterType<UrlSessionDownloadDelegate, SpecificUrlSessionDownloadDelegate>();

            Mvx.IoCProvider.LazyConstructAndRegisterSingleton<IDownloadManager, IosDownloadManager>();

            Mvx.IoCProvider.RegisterDecorator<IUriOpener, InternalLinksOpener, UriOpener>();

            Mvx.IoCProvider.LazyConstructAndRegisterSingleton<IFileDownloader, IosFileDownloader>();

            Mvx.IoCProvider.RegisterType<IBrowser, BrowserSelector>();

            RegisterMediaPlayer();
        }

        public override void InitializeSecondary()
        {
            base.InitializeSecondary();
            InitializeImageService();
        }

        private static void InitializeImageService()
        {
            ImageService.Instance.Initialize(new FFImageLoading.Config.Configuration
            {
                InvalidateLayout = false,
                HttpClient = new HttpClient(new AuthenticatedNativeHttpImageClientHandler(Mvx.IoCProvider.Resolve<IMediaRequestHttpHeaders>()))
            });
        }

        private void RegisterMediaPlayer()
        {
            Mvx.IoCProvider.RegisterType<IMediaPlayerInitializer>(Mvx.IoCProvider.Resolve<ICommandCenter>);
            Mvx.IoCProvider.LazyConstructAndRegisterSingleton<IPlatformSpecificMediaPlayer, IosMediaPlayer>();
            Mvx.IoCProvider.LazyConstructAndRegisterSingleton<IMediaRemoteControl, MediaRemoteControl>();

            Mvx.IoCProvider.RegisterType<AvPlayerItemFactory>();
            Mvx.IoCProvider.LazyConstructAndRegisterSingleton<IAudioPlayback, AVAudioPlayback>();
            Mvx.IoCProvider.LazyConstructAndRegisterSingleton<ICommandCenter, SeekableRemoteCommandCenter>();
            Mvx.IoCProvider.LazyConstructAndRegisterSingleton<MediaQueue, MediaQueue>();
            Mvx.IoCProvider.LazyConstructAndRegisterSingleton<IShuffleableQueue>(() => new ShuffleableQueue(Mvx.IoCProvider.Resolve<MediaQueue>(), Mvx.IoCProvider.Resolve<ILogger>()));
            Mvx.IoCProvider.RegisterType<IMediaQueue>(Mvx.IoCProvider.Resolve<IShuffleableQueue>);
        }

        protected override IMvxApplication CreateApp()
        {
            return new App();
        }

        protected override IMvxIosViewPresenter CreateViewPresenter()
        {
            var presenter = new Presenter(ApplicationDelegate, Window);
            Mvx.IoCProvider.RegisterSingleton<IViewModelAwareViewPresenter>(presenter);
            return presenter;
        }

        protected override IMvxIocOptions CreateIocOptions()
        {
            return new MvxIocOptions
            {
                PropertyInjectorOptions = MvxPropertyInjectorOptions.MvxInject
            };
        }
    }
}