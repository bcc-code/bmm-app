using Acr.UserDialogs;
using BMM.Api.Abstraction;
using BMM.Api.Framework;
using BMM.Api.Framework.HTTP;
using BMM.Core;
using BMM.Core.Constants;
using BMM.Core.GuardedActions.TrackOptions.Interfaces;
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
using BMM.Core.Support;
using BMM.UI.iOS.Actions;
using BMM.UI.iOS.Bindings;
using BMM.UI.iOS.DownloadManager;
using BMM.UI.iOS.Helpers;
using BMM.UI.iOS.Implementations;
using BMM.UI.iOS.Implementations.Device;
using BMM.UI.iOS.Implementations.Dialogs;
using BMM.UI.iOS.Implementations.Download;
using BMM.UI.iOS.Implementations.Notifications;
using BMM.UI.iOS.Implementations.Support;
using BMM.UI.iOS.Implementations.Track;
using BMM.UI.iOS.Implementations.UI;
using BMM.UI.iOS.NewMediaPlayer;
using BMM.UI.iOS.NewMediaPlayer.Interfaces;
using BMM.UI.iOS.UI;
using FFImageLoading;
using FFImageLoading.Cache;
using FFImageLoading.Config;
using IdentityModel.OidcClient.Browser;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Storage;
using MvvmCross;
using MvvmCross.Base;
using MvvmCross.Binding.Bindings.Target.Construction;
using MvvmCross.IoC;
using MvvmCross.Platforms.Ios.Core;
using MvvmCross.Platforms.Ios.Presenters;
using MvvmCross.ViewModels;
using Serilog;
using Serilog.Extensions.Logging;
using ILogger = BMM.Api.Framework.ILogger;

namespace BMM.UI.iOS
{
    public class IosSetup : MvxIosSetup<App>
    {
        private const int ImageServiceTimeoutInSeconds = 300;
        
        protected override void InitializeFirstChance(IMvxIoCProvider iocProvider)
        {
            base.InitializeFirstChance(iocProvider);

            // Since we need Firebase already in the dependency injection initialization we have to configure it here.
            // FinishedLaunching of the AppDelegate (as suggested in the Firebase docs) would be to late
            // because it's called after creating the DI containers in Mvx.
            if (Firebase.Core.App.DefaultInstance == null)
                Firebase.Core.App.Configure();

            iocProvider.LazyConstructAndRegisterSingleton<IOldSecureStorage, iOSOldSecureStorage>();
            iocProvider.LazyConstructAndRegisterSingleton<IUserDialogsFactory, iOSUserDialogsFactory>();
            iocProvider.LazyConstructAndRegisterSingleton<IStopwatchManager, StopwatchManager>();
            var stopwatch = iocProvider.Resolve<IStopwatchManager>();
            stopwatch.StartAndGetStopwatch(StopwatchType.AppStart);

            iocProvider.RegisterType<INotificationSubscriptionTokenProvider, FirebaseTokenProvider>();

#if DEBUG && !UI_TESTS
            iocProvider.RegisterType<ILogger>(
                () => new ErrorDialogDisplayingLogger(
                    iocProvider.Resolve<IUserDialogsFactory>().Create(),
                    new IosLogger(iocProvider.Resolve<IUserStorage>(), iocProvider.Resolve<IConnection>()),
                    iocProvider.Resolve<IMvxMainThreadAsyncDispatcher>())
            );
#else
            iocProvider.RegisterType<ILogger, IosLogger>();
#endif

            iocProvider.LazyConstructAndRegisterSingleton<IUserDialogs, iOSExceptionHandlingUserDialogs>();
            iocProvider.ConstructAndRegisterSingleton<IUiDependentExecutor, BottomNavigationLoadedDependentExecutor>();
            iocProvider.RegisterType<ISimpleHttpClient, NetworkAccessAwareHttpClient>();
            iocProvider.LazyConstructAndRegisterSingleton<ITrackMediaHelper, TrackMediaHelper>();
            iocProvider.LazyConstructAndRegisterSingleton<IStorageManager, StorageManager>();
            iocProvider.LazyConstructAndRegisterSingleton<INotificationDisplayer, NotificationDisplayer>();
            iocProvider.RegisterType<UserNotificationCenterDelegate>();
            iocProvider.RegisterType<IClipboardService, ClipboardService>();
            iocProvider.RegisterType<ITrackOptionsService, iOSTrackOptionsService>();
            iocProvider.RegisterType<IBMMUserDialogs, iOSBMMUserDialogs>();
            iocProvider.LazyConstructAndRegisterSingleton<IPlatformSpecificRemoteConfig, IosFirebaseRemoteConfig>();
            iocProvider.LazyConstructAndRegisterSingleton<IDeviceSupportVersionChecker, IosSupportVersionChecker>();

            iocProvider.RegisterType<UrlSessionDownloadDelegate, SpecificUrlSessionDownloadDelegate>();

            iocProvider.LazyConstructAndRegisterSingleton<IDownloadManager, IosDownloadManager>();

            iocProvider.RegisterDecorator<IUriOpener, InternalLinksOpener, UriOpener>();

            iocProvider.LazyConstructAndRegisterSingleton<IFileDownloader, IosFileDownloader>();

            iocProvider.RegisterType<IBrowser, BrowserSelector>();
            
            iocProvider.LazyConstructAndRegisterSingleton<IFeatureSupportInfoService, iOSFeatureSupportInfoService>();
            iocProvider.RegisterType<INotificationPermissionService, iOSNotificationPermissionService>();
            iocProvider.LazyConstructAndRegisterSingleton<IDialogService, iOSDialogService>();
            iocProvider.LazyConstructAndRegisterSingleton<IDeviceInfo, iOSDeviceInfo>();
            
            iocProvider.RegisterType<IPrepareTrackOptionsAction, iOSTrackOptionsAction>();
            
            RegisterMediaPlayer(iocProvider);
        }

        protected override ILoggerProvider CreateLogProvider()
        {
            return new SerilogLoggerProvider();
        }

        protected override ILoggerFactory CreateLogFactory()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .CreateLogger();

            return new SerilogLoggerFactory();
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
                HttpHeadersTimeout = ImageServiceTimeoutInSeconds,
                HttpReadTimeout = ImageServiceTimeoutInSeconds,
                HttpClient = new HttpClient(new AuthenticatedHttpImageClientHandler(Mvx.IoCProvider.Resolve<IMediaRequestHttpHeaders>()))
                {
                    Timeout = TimeSpan.FromSeconds(ImageServiceTimeoutInSeconds),
                },
                DiskCache = new SimpleDiskCache(Path.Combine(FileSystem.AppDataDirectory, ImageServiceConstants.ImageCacheFolder), new Configuration
                {
                    DiskCacheDuration = ImageServiceConstants.DiskCacheDuration
                }),
                MaxMemoryCacheSize = ImageServiceConstants.ImageCacheMemorySize
            });
        }
        

        private void RegisterMediaPlayer(IMvxIoCProvider iocProvider)
        {
            iocProvider.RegisterType<IMediaPlayerInitializer>(iocProvider.Resolve<ICommandCenter>);
            iocProvider.LazyConstructAndRegisterSingleton<IPlatformSpecificMediaPlayer, IosMediaPlayer>();
            iocProvider.LazyConstructAndRegisterSingleton<IMediaRemoteControl, MediaRemoteControl>();
            iocProvider.LazyConstructAndRegisterSingleton<IAVPlayerItemRepository, AVPlayerItemRepository>();
            iocProvider.LazyConstructAndRegisterSingleton<ICacheAVPlayerItemLoaderFactory, CacheAVPlayerItemLoaderFactory>();
            iocProvider.RegisterType<IAVPlayerItemFactory, AVPlayerItemFactory>();
            iocProvider.LazyConstructAndRegisterSingleton<IAudioPlayback, AVAudioPlayback>();
            iocProvider.LazyConstructAndRegisterSingleton<ICommandCenter, SeekableRemoteCommandCenter>();
            iocProvider.LazyConstructAndRegisterSingleton<MediaQueue, MediaQueue>();
            iocProvider.LazyConstructAndRegisterSingleton<IShuffleableQueue>(() => new ShuffleableQueue(iocProvider.Resolve<MediaQueue>(), iocProvider.Resolve<ILogger>()));
            iocProvider.RegisterType<IMediaQueue>(iocProvider.Resolve<IShuffleableQueue>);
        }

        protected override void FillTargetFactories(IMvxTargetBindingFactoryRegistry registry)
        {
            base.FillTargetFactories(registry);
            UIButtonEnabledBinding.Register(registry);
        }
        
        protected override IMvxApplication CreateApp(IMvxIoCProvider iocProvider)
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