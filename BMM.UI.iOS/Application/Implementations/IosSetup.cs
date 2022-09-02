using System;
using System.IO;
using System.Net.Http;
using Acr.UserDialogs;
using BMM.Api.Abstraction;
using BMM.Api.Framework;
using BMM.Api.Framework.HTTP;
using BMM.Core;
using BMM.Core.Constants;
using BMM.Core.Helpers;
using BMM.Core.Implementations;
using BMM.Core.Implementations.Analytics;
using BMM.Core.Implementations.Device;
using BMM.Core.Implementations.Dialogs;
using BMM.Core.Implementations.Downloading.FileDownloader;
using BMM.Core.Implementations.DownloadManager;
using BMM.Core.Implementations.Exceptions;
using BMM.Core.Implementations.FileStorage;
using BMM.Core.Implementations.FirebaseRemoteConfig;
using BMM.Core.Implementations.Networking;
using BMM.Core.Implementations.Notifications;
using BMM.Core.Implementations.Security;
using BMM.Core.Implementations.Tracks.Interfaces;
using BMM.Core.Implementations.UI;
using BMM.Core.NewMediaPlayer;
using BMM.Core.NewMediaPlayer.Abstractions;
using BMM.UI.iOS.DownloadManager;
using BMM.UI.iOS.Helpers;
using BMM.UI.iOS.Implementations;
using BMM.UI.iOS.Implementations.Device;
using BMM.UI.iOS.Implementations.Dialogs;
using BMM.UI.iOS.Implementations.Download;
using BMM.UI.iOS.Implementations.Notifications;
using BMM.UI.iOS.Implementations.Track;
using BMM.UI.iOS.NewMediaPlayer;
using BMM.UI.iOS.NewMediaPlayer.Interfaces;
using BMM.UI.iOS.UI;
using FFImageLoading;
using FFImageLoading.Cache;
using FFImageLoading.Config;
using IdentityModel.OidcClient.Browser;
using MvvmCross;
using MvvmCross.Base;
using MvvmCross.IoC;
using MvvmCross.Platforms.Ios.Core;
using MvvmCross.Platforms.Ios.Presenters;
using MvvmCross.ViewModels;
using Xamarin.Essentials;

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

            Mvx.IoCProvider.LazyConstructAndRegisterSingleton<IUserDialogsFactory, iOSUserDialogsFactory>();
            Mvx.IoCProvider.LazyConstructAndRegisterSingleton<IStopwatchManager, StopwatchManager>();
            var stopwatch = Mvx.IoCProvider.Resolve<IStopwatchManager>();
            stopwatch.StartAndGetStopwatch(StopwatchType.AppStart);

            Mvx.IoCProvider.RegisterType<INotificationSubscriptionTokenProvider, FirebaseTokenProvider>();

#if DEBUG && !UI_TESTS
            Mvx.IoCProvider.RegisterType<ILogger>(
                () => new ErrorDialogDisplayingLogger(
                    Mvx.IoCProvider.Resolve<IUserDialogsFactory>().Create(),
                    new IosLogger(Mvx.IoCProvider.Resolve<IUserStorage>(), Mvx.IoCProvider.Resolve<IConnection>()),
                    Mvx.IoCProvider.Resolve<IMvxMainThreadAsyncDispatcher>())
            );
#else
            Mvx.IoCProvider.RegisterType<ILogger, IosLogger>();
#endif

            Mvx.IoCProvider.LazyConstructAndRegisterSingleton<IUserDialogs, iOSExceptionHandlingUserDialogs>();
            Mvx.IoCProvider.ConstructAndRegisterSingleton<IUiDependentExecutor, BottomNavigationLoadedDependentExecutor>();
            Mvx.IoCProvider.RegisterType<ISimpleHttpClient, NetworkAccessAwareHttpClient>();
            Mvx.IoCProvider.LazyConstructAndRegisterSingleton<ITrackMediaHelper, TrackMediaHelper>();
            Mvx.IoCProvider.LazyConstructAndRegisterSingleton<IStorageManager, StorageManager>();
            Mvx.IoCProvider.LazyConstructAndRegisterSingleton<INotificationDisplayer, NotificationDisplayer>();
            Mvx.IoCProvider.RegisterType<UserNotificationCenterDelegate>();
            Mvx.IoCProvider.RegisterType<IClipboardService, ClipboardService>();
            Mvx.IoCProvider.RegisterType<ITrackOptionsService, iOSTrackOptionsService>();
            Mvx.IoCProvider.RegisterType<IBMMUserDialogs, iOSBMMUserDialogs>();
            Mvx.IoCProvider.LazyConstructAndRegisterSingleton<IPlatformSpecificRemoteConfig, IosFirebaseRemoteConfig>();
            Mvx.IoCProvider.LazyConstructAndRegisterSingleton<IDeviceSupportVersionChecker, IosSupportVersionChecker>();

            Mvx.IoCProvider.RegisterType<UrlSessionDownloadDelegate, SpecificUrlSessionDownloadDelegate>();

            Mvx.IoCProvider.LazyConstructAndRegisterSingleton<IDownloadManager, IosDownloadManager>();

            Mvx.IoCProvider.RegisterDecorator<IUriOpener, InternalLinksOpener, UriOpener>();

            Mvx.IoCProvider.LazyConstructAndRegisterSingleton<IFileDownloader, IosFileDownloader>();

            Mvx.IoCProvider.RegisterType<IBrowser, BrowserSelector>();
            
            Mvx.IoCProvider.LazyConstructAndRegisterSingleton<IFeatureSupportInfoService, iOSFeatureSupportInfoService>();
            RegisterMediaPlayer();
        }

        public override void InitializeSecondary()
        {
            base.InitializeSecondary();
            InitializeImageService();
        }

        private static void InitializeImageService()
        {
            ImageService.Instance.Initialize(new Configuration
            {
                InvalidateLayout = false,
                HttpClient = new HttpClient(new AuthenticatedHttpImageClientHandler(Mvx.IoCProvider.Resolve<IMediaRequestHttpHeaders>())),
                DiskCache = new SimpleDiskCache(Path.Combine(FileSystem.AppDataDirectory, ImageServiceConstants.ImageCacheFolder), new Configuration
                {
                    DiskCacheDuration = ImageServiceConstants.DiskCacheDuration
                }),
                MaxMemoryCacheSize = ImageServiceConstants.ImageCacheMemorySize
            });
        }

        private void RegisterMediaPlayer()
        {
            Mvx.IoCProvider.RegisterType<IMediaPlayerInitializer>(Mvx.IoCProvider.Resolve<ICommandCenter>);
            Mvx.IoCProvider.LazyConstructAndRegisterSingleton<IPlatformSpecificMediaPlayer, IosMediaPlayer>();
            Mvx.IoCProvider.LazyConstructAndRegisterSingleton<IMediaRemoteControl, MediaRemoteControl>();
            Mvx.IoCProvider.LazyConstructAndRegisterSingleton<IAVPlayerItemRepository, AVPlayerItemRepository>();
            
            Mvx.IoCProvider.RegisterType<IAVPlayerItemFactory, AVPlayerItemFactory>();
            Mvx.IoCProvider.RegisterType<ICacheAVPlayerItemFactory, CacheAVPlayerItemFactory>();
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