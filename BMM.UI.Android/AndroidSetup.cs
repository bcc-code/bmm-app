using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Reflection;
using Acr.UserDialogs;
using Android.OS;
using Android.Runtime;
using Android.Views;
using AndroidX.CardView.Widget;
using AndroidX.DrawerLayout.Widget;
using AndroidX.ViewPager.Widget;
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
using BMM.Core.Implementations.DownloadManager;
using BMM.Core.Implementations.Exceptions;
using BMM.Core.Implementations.FileStorage;
using BMM.Core.Implementations.FirebaseRemoteConfig;
using BMM.Core.Implementations.Networking;
using BMM.Core.Implementations.Notifications;
using BMM.Core.Implementations.Player;
using BMM.Core.Implementations.PlayObserver.Streak;
using BMM.Core.Implementations.Security;
using BMM.Core.Implementations.Tracks.Interfaces;
using BMM.Core.Implementations.UI;
using BMM.Core.NewMediaPlayer;
using BMM.Core.NewMediaPlayer.Abstractions;
using BMM.Core.Support;
using BMM.UI.Droid.Application.Bindings;
using BMM.UI.Droid.Application.DownloadManager;
using BMM.UI.Droid.Application.Helpers;
using BMM.UI.Droid.Application.Implementations;
using BMM.UI.Droid.Application.Implementations.App;
using BMM.UI.Droid.Application.Implementations.Device;
using BMM.UI.Droid.Application.Implementations.Dialogs;
using BMM.UI.Droid.Application.Implementations.FileStorage;
using BMM.UI.Droid.Application.Implementations.FirebaseRemoteConfig;
using BMM.UI.Droid.Application.Implementations.Networking;
using BMM.UI.Droid.Application.Implementations.Notifications;
using BMM.UI.Droid.Application.Implementations.Oidc;
using BMM.UI.Droid.Application.Implementations.Support;
using BMM.UI.Droid.Application.Implementations.Track;
using BMM.UI.Droid.Application.Implementations.UI;
using BMM.UI.Droid.Application.Media;
using BMM.UI.Droid.Application.NewMediaPlayer;
using BMM.UI.Droid.Application.NewMediaPlayer.Controller;
using BMM.UI.Droid.Application.NewMediaPlayer.Playback;
using BMM.UI.iOS.UI;
using Com.Google.Android.Exoplayer2.Ext.Mediasession;
using FFImageLoading;
using FFImageLoading.Cache;
using FFImageLoading.Config;
using Google.Android.Material.FloatingActionButton;
using Google.Android.Material.Navigation;
using IdentityModel.OidcClient.Browser;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Storage;
using MvvmCross;
using MvvmCross.Base;
using MvvmCross.Binding.Bindings.Target.Construction;
using MvvmCross.DroidX;
using MvvmCross.DroidX.RecyclerView;
using MvvmCross.IoC;
using MvvmCross.Platforms.Android.Core;
using MvvmCross.Platforms.Android.Presenters;
using MvvmCross.Plugin.Color.Platforms.Android.BindingTargets;
using MvvmCross.Plugin.ResourceLoader.Platforms.Android;
using MvvmCross.ViewModels;
using Serilog;
using Serilog.Extensions.Logging;
using ILogger = BMM.Api.Framework.ILogger;
using Log = Android.Util.Log;
using Toolbar = AndroidX.AppCompat.Widget.Toolbar;
using TrackMediaHelper = BMM.UI.Droid.Application.Implementations.Media.TrackMediaHelper;

namespace BMM.UI.Droid
{
    public class AndroidSetup : MvxAndroidSetup<App>
    {
        private const int ImageServiceTimeoutInSeconds = 300;
        
        protected override IMvxApplication CreateApp(IMvxIoCProvider iocProvider)
        {
            AndroidEnvironment.UnhandledExceptionRaiser += (sender, args) =>
            {
                LogError("AndroidEnvironment.UnhandledException", args.Exception.ToString());
            };
            AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
            {
                LogError("AppDomain.CurrentDomain.UnhandledException", args.ExceptionObject.ToString());
            };

            return new App();
        }

        private void LogError(string location, string exception)
        {
#if DEBUG
            Log.Error(location, exception);
#endif
        }

        protected override IMvxIocOptions CreateIocOptions()
        {
            return new MvxIocOptions
            {
                PropertyInjectorOptions = MvxPropertyInjectorOptions.MvxInject
            };
        }

        protected override IMvxAndroidViewPresenter CreateViewPresenter()
        {
            var customPresenter = new ViewModelAwareViewPresenter(AndroidViewAssemblies);
            Mvx.IoCProvider.RegisterSingleton<IMvxAndroidViewPresenter>(customPresenter);
            Mvx.IoCProvider.RegisterSingleton<IViewModelAwareViewPresenter>(customPresenter);
            return customPresenter;
        }

        protected override IEnumerable<Assembly> AndroidViewAssemblies
        {
            get
            {
                var assemblies = new List<Assembly>(base.AndroidViewAssemblies)
                {
                    typeof(NavigationView).Assembly,
                    typeof(FloatingActionButton).Assembly,
                    typeof(Toolbar).Assembly,
                    typeof(CardView).Assembly,
                    typeof(DrawerLayout).Assembly,
                    typeof(ViewPager).Assembly,
                    typeof(MvxRecyclerView).Assembly,
                    typeof(MvxSwipeRefreshLayout).Assembly
                };

                return assemblies;
            }
        }

        protected override void InitializeFirstChance(IMvxIoCProvider iocProvider)
        {
            base.InitializeFirstChance(iocProvider);

            iocProvider.LazyConstructAndRegisterSingleton<IOldSecureStorage, DroidOldSecureStorage>();
            iocProvider.RegisterType<IMvxResourceLoader, MvxAndroidResourceLoader>();
            iocProvider.LazyConstructAndRegisterSingleton<IUserDialogsFactory, DroidUserDialogsFactory>();

            iocProvider.LazyConstructAndRegisterSingleton<IStopwatchManager, StopwatchManager>();
            var stopwatch = iocProvider.Resolve<IStopwatchManager>();
            stopwatch.StartAndGetStopwatch(StopwatchType.AppStart);

            Firebase.FirebaseApp.InitializeApp(ApplicationContext);

            iocProvider.RegisterType<INotificationSubscriptionTokenProvider, FirebaseTokenProvider>();
#if DEBUG
            iocProvider.RegisterType<ILogger>(
                () => new ErrorDialogDisplayingLogger(iocProvider.Resolve<IUserDialogsFactory>().Create(),
                    new AndroidLogger(iocProvider.Resolve<IUserStorage>(), iocProvider.Resolve<IConnection>()),
                    iocProvider.Resolve<IMvxMainThreadAsyncDispatcher>()));
#else
            iocProvider.RegisterType<ILogger, AndroidLogger>();
#endif
            
            iocProvider.LazyConstructAndRegisterSingleton<IUserDialogs, DroidExceptionHandlingUserDialogs>();
            iocProvider.RegisterType<IClipboardService, ClipboardService>();
            iocProvider.RegisterType<ITrackOptionsService, DroidTrackOptionsService>();
            iocProvider.RegisterType<IBMMUserDialogs, DroidBMMUserDialogs>();

            iocProvider.RegisterSingleton<ISdkVersionHelper>(new SdkVersionHelper(Build.VERSION.SdkInt));
            iocProvider.RegisterType<ISimpleHttpClient, SimpleHttpClient>();
            iocProvider.LazyConstructAndRegisterSingleton<ITrackMediaHelper, TrackMediaHelper>();
            iocProvider.LazyConstructAndRegisterSingleton<IDownloadManager, AndroidDownloadManager>();
            iocProvider.LazyConstructAndRegisterSingleton<IStorageManager, StorageManager>();
            iocProvider.LazyConstructAndRegisterSingleton<INotificationDisplayer, NotificationDisplayer>();
            iocProvider.LazyConstructAndRegisterSingleton<IPlatformSpecificRemoteConfig, AndroidFirebaseRemoteConfig>();
            iocProvider.LazyConstructAndRegisterSingleton<IDeviceSupportVersionChecker, AndroidSupportVersionChecker>();

            iocProvider.RegisterDecorator<IUriOpener, InternalLinksOpener, UriOpener>();
            iocProvider.RegisterType<DownloadCompletedHandler, DownloadCompletedHandler>();
            iocProvider.RegisterType<MediaMountedHandler, MediaMountedHandler>();

            iocProvider.RegisterType<IBrowser, BrowserSelector>();
            iocProvider.RegisterType<IFeatureSupportInfoService, DroidFeaturePreviewPermission>();
            iocProvider.RegisterType<INotificationPermissionService, DroidNotificationPermissionService>();
            iocProvider.RegisterType<ISystemSettingsService, SystemSettingsService>();
            iocProvider.LazyConstructAndRegisterSingleton<IDialogService, DroidDialogService>();
            iocProvider.LazyConstructAndRegisterSingleton<IDeviceInfo, DroidDeviceInfo>();
            
            InitializeMediaPlayer(iocProvider);
        }
        
        protected override ILoggerProvider CreateLogProvider()
        {
            return new SerilogLoggerProvider();
        }

        protected override ILoggerFactory CreateLogFactory()
        {
            Serilog.Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .CreateLogger();

            return new SerilogLoggerFactory();
        }

        private static void InitializeImageService()
        {
            ImageService.Instance.Initialize(new Configuration
            {
                HttpHeadersTimeout = ImageServiceTimeoutInSeconds,
                HttpReadTimeout = ImageServiceTimeoutInSeconds,
                InvalidateLayout = false,
                HttpClient = new HttpClient(
                    new DroidAuthenticatedHttpImageClientHandler(Mvx.IoCProvider.Resolve<IMediaRequestHttpHeaders>())
                    {
                        ConnectTimeout = TimeSpan.FromSeconds(ImageServiceTimeoutInSeconds),
                        AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
                    }),
                DiskCache = new SimpleDiskCache(Path.Combine(FileSystem.AppDataDirectory, ImageServiceConstants.ImageCacheFolder), new Configuration
                {
                    DiskCacheDuration = ImageServiceConstants.DiskCacheDuration
                }),
                MaxMemoryCacheSize = ImageServiceConstants.ImageCacheMemorySize
            });
        }

        protected override void FillTargetFactories(IMvxTargetBindingFactoryRegistry registry)
        {
            base.FillTargetFactories(registry);
            
            registry.RegisterFactory(new MvxCustomBindingFactory<View>("BackgroundTint", view => new MvxBackgroundTintBinding(view)));
            registry.RegisterFactory(new MvxCustomBindingFactory<CardView>("CardVisibility", card => new MvxCardVisibility(card)));
            registry.RegisterFactory(new MvxCustomBindingFactory<CardView>("CardCircle", card => new MvxCardCircle(card)));
            registry.RegisterFactory(new MvxCustomBindingFactory<CardView>("CardBackgroundColor", card => new MvxCardBackgroundColor(card)));
            MvxCachedImageViewPathBinding.Register(registry);
            BackgroundResourceBinding.Register(registry);
            ImageButtonIconResourceBinding.Register(registry);
            AlphaTargetBinding.Register(registry);
            IsEnabledBinding.Register(registry);
            HexMvxCardBackgroundColor.Register(registry);
        }

        private void InitializeMediaPlayer(IMvxIoCProvider iocProvider)
        {
            iocProvider.RegisterType<IMediaPlayerInitializer, NullMediaPlayerInitializer>();
            iocProvider.LazyConstructAndRegisterSingleton<IMediaQueue, MediaQueue>();
            iocProvider.RegisterType<MediaControllerCallback, MediaControllerCallback>();
            iocProvider.RegisterType<PeriodicExecutor, PeriodicExecutor>();
            iocProvider.RegisterType<PlaybackStateCompatMapper, PlaybackStateCompatMapper>();
            iocProvider.RegisterType<MediaSessionConnector.IPlaybackPreparer, ExoPlaybackPreparer>();

            iocProvider.RegisterType<NotificationChannelBuilder>();
            iocProvider.RegisterType<IMetadataMapper, MetadataMapper>();
            iocProvider.LazyConstructAndRegisterSingleton<IPlatformSpecificMediaPlayer, AndroidMediaPlayer>();
        }

        public override void InitializeSecondary()
        {
            try
            {
                base.InitializeSecondary();
                InitializeImageService();
            }
            catch (Exception e)
            {
                // Since the exception is thrown on a background thread we need to move it to the main thread.
                // On the main thread it makes the app crash as expected
                // This is needed to make the app crash when something goes wrong at AppStart. E.g. MvxIoCResolveException
                var dispatcher = Mvx.IoCProvider.Resolve<IMvxMainThreadAsyncDispatcher>();
                dispatcher.ExecuteOnMainThreadAsync(() => { throw e; });

                throw;
            }
        }
    }
}