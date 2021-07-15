using System;
using System.Net.Http;
using Acr.UserDialogs;
using Akavache;
using BMM.Api;
using BMM.Api.Abstraction;
using BMM.Api.Framework;
using BMM.Api.Framework.HTTP;
using BMM.Api.Implementation;
using BMM.Api.Implementation.Clients;
using BMM.Api.Implementation.Clients.Contracts;
using BMM.Api.RequestInterceptor;
using BMM.Core.Helpers;
using BMM.Core.Implementations;
using BMM.Core.Implementations.Analytics;
using BMM.Core.Implementations.ApiClients;
using BMM.Core.Implementations.Caching;
using BMM.Core.Implementations.Connection;
using BMM.Core.Implementations.DeepLinking;
using BMM.Core.Implementations.Device;
using BMM.Core.Implementations.DocumentFilters;
using BMM.Core.Implementations.Downloading;
using BMM.Core.Implementations.Downloading.DownloadQueue;
using BMM.Core.Implementations.Downloading.FileDownloader;
using BMM.Core.Implementations.Exceptions;
using BMM.Core.Implementations.FeatureToggles;
using BMM.Core.Implementations.Feedback;
using BMM.Core.Implementations.FileStorage;
using BMM.Core.Implementations.FileStorage.StreamToFileSystemWriter;
using BMM.Core.Implementations.FirebaseRemoteConfig;
using BMM.Core.Implementations.Languages;
using BMM.Core.Implementations.LiveRadio;
using BMM.Core.Implementations.Localization;
using BMM.Core.Implementations.Media;
using BMM.Core.Implementations.Notifications;
using BMM.Core.Implementations.Notifications.Data;
using BMM.Core.Implementations.Player;
using BMM.Core.Implementations.PlaylistPersistence;
using BMM.Core.Implementations.PlayObserver;
using BMM.Core.Implementations.PlayObserver.Storage;
using BMM.Core.Implementations.PlayObserver.Streak;
using BMM.Core.Implementations.Podcasts;
using BMM.Core.Implementations.PostLoginActions;
using BMM.Core.Implementations.Security;
using BMM.Core.Implementations.Security.Oidc;
using BMM.Core.Implementations.Serialization;
using BMM.Core.Implementations.Startup;
using BMM.Core.Implementations.TrackCollections;
using BMM.Core.Implementations.TrackListenedObservation;
using BMM.Core.Implementations.UI;
using BMM.Core.Implementations.Validators;
using BMM.Core.Messages;
using BMM.Core.NewMediaPlayer;
using BMM.Core.NewMediaPlayer.Abstractions;
using FFImageLoading;
using MvvmCross;
using MvvmCross.Base;
using MvvmCross.IoC;
using MvvmCross.Localization;
using MvvmCross.Plugin.JsonLocalization;
using MvvmCross.Plugin.Messenger;
using MvvmCross.ViewModels;

// ReSharper disable RedundantTypeArgumentsOfMethod

namespace BMM.Core
{
    public class App : MvxApplication
    {
        /// <summary>
        /// Called once at startup to initialize classes and start the app
        /// </summary>
        public override void Initialize()
        {
            var serverUri = new ApiBaseUri(GlobalConstants.ApiUrl);

            Registrations.Start(GlobalConstants.ApplicationName);
            BlobCache.ApplicationName = GlobalConstants.PackageName;

            Mvx.IoCProvider.RegisterType<IDeviceInfo, DeviceInfo>();
            Mvx.IoCProvider.RegisterTypeIfMissing<IUiDependentExecutor, NullDependentExecutor>();
            Mvx.IoCProvider.RegisterType<IFirebaseRemoteConfig, FirebaseRemoteConfig>();
            Mvx.IoCProvider.RegisterType<IFeaturePreviewPermission, PermissionProvider>();
            Mvx.IoCProvider.RegisterType<IDeveloperPermission, PermissionProvider>();
            Mvx.IoCProvider.RegisterType<SemanticVersionComparer>();
            Mvx.IoCProvider.RegisterType<SemanticVersionParser>();
            Mvx.IoCProvider.RegisterType<AppSupportVersionChecker>();
            Mvx.IoCProvider.RegisterType<SupportVersionChecker>();
            Mvx.IoCProvider.RegisterType<IBadRequestThrower, BadRequestThrower>();
            Mvx.IoCProvider.RegisterType<IResponseDeserializer, ResponseDeserializer>();
            Mvx.IoCProvider.RegisterType<IBmmVersionProvider, BmmVersionProvider>();

            Mvx.IoCProvider.RegisterType<IAnalytics, AppCenterAnalytics>();
            Mvx.IoCProvider.RegisterType<IDeepLinkHandler, DeepLinkHandler>();
            Mvx.IoCProvider.RegisterType<IShareLink, ShareLink>();
            Mvx.IoCProvider.RegisterType<IAppNavigator, AppNavigator>();
            Mvx.IoCProvider.RegisterTypeIfMissing<ICredentialsStorage, AkavacheCredentialsStorage>();

            if (!Mvx.IoCProvider.CanResolve<IUserStorage>())
            {
                Mvx.IoCProvider.LazyConstructAndRegisterSingleton<IUserStorage, AkavacheUserStorage>();
            }

            Mvx.IoCProvider.RegisterType<IProfileLoader, ProfileLoader>();
            Mvx.IoCProvider.RegisterType<ISecureStorageProxy, SecureStorageProxy>();
            Mvx.IoCProvider.RegisterType<IOidcCredentialsStorage, OidcCredentialsStorage>();

            Mvx.IoCProvider.RegisterSingleton(new HttpClient
            {
                Timeout = new TimeSpan(GlobalConstants.NetworkRequestTimeout * TimeSpan.TicksPerSecond)
            });

            // By default the UserAccount BlobCache is used. If LocalMachine is needed manual creation is needed
            Mvx.IoCProvider.RegisterSingleton<IBlobCache>(BlobCache.UserAccount);

            Mvx.IoCProvider.LazyConstructAndRegisterSingleton<INotificationCenter, NotificationCenter>();

            Mvx.IoCProvider.LazyConstructAndRegisterSingleton<ISettingsStorage, AkavacheBlobSettingsStorage>();

            Mvx.IoCProvider.RegisterType<INetworkSettings, NetworkSettings>();
            Mvx.IoCProvider.RegisterType<IMvxJsonConverter, MvxJsonConverter>();

            Mvx.IoCProvider.LazyConstructAndRegisterSingleton<IConnection, XamarinEssentialsConnection>();

            Mvx.IoCProvider.LazyConstructAndRegisterSingleton<IExceptionHandler>(

                // Use direct instance of UserDialog.Instance to avoid nested-loops with ExceptionHandlingUserDialogs
                () => new ExceptionHandler(
                    new ToastDisplayer(UserDialogs.Instance),
                    Mvx.IoCProvider.Resolve<ILogger>(),
                    Mvx.IoCProvider.Resolve<IAnalytics>())
            );

            Mvx.IoCProvider.ConstructAndRegisterSingleton<BackgroundTaskExecutor, BackgroundTaskExecutor>();
            Mvx.IoCProvider.LazyConstructAndRegisterSingleton<IUserDialogs, ExceptionHandlingUserDialogs>();
            Mvx.IoCProvider.LazyConstructAndRegisterSingleton<IToastDisplayer, ToastDisplayer>();

            Mvx.IoCProvider.RegisterDecorator<IContacter, ErrorCatchingContacterDecorator, EmailContacter>();
            Mvx.IoCProvider.RegisterType<IDocumentSerializer, DocumentSerializer>();
            Mvx.IoCProvider.RegisterType<IMailValidator, MailValidator>();

            Mvx.IoCProvider.RegisterType<EnvironmentLanguageReader>();
            Mvx.IoCProvider.LazyConstructAndRegisterSingleton<IContentLanguageManager, ContentLanguageManager>();
            Mvx.IoCProvider.RegisterType<IAppLanguageProvider, AppLanguageProvider>();

            Mvx.IoCProvider.LazyConstructAndRegisterSingleton<IAccessTokenProvider, AccessTokenProvider>();
            Mvx.IoCProvider.LazyConstructAndRegisterSingleton<IAuthorizationHeaderProvider, BearerTokenAuthorizationHeaderProvider>();

            Mvx.IoCProvider.RegisterType<IMediaRequestHttpHeaders, MediaRequestHttpHeaders>();
            Mvx.IoCProvider.RegisterType<IRequestHandlerFactory, RequestHandlerFactory>();
            Mvx.IoCProvider.RegisterType<HttpHeaderProviders.UnauthorizedRequests>();
            Mvx.IoCProvider.RegisterType<HttpHeaderProviders.AuthorizedRequests>();
            Mvx.IoCProvider.RegisterType<HttpHeaderProviders.MediaRequests>();
            Mvx.IoCProvider.RegisterType<ContentLanguageHeaderProvider>();
            Mvx.IoCProvider.RegisterType<JsonContentTypeHeaderProvider>();
            Mvx.IoCProvider.RegisterType<BmmVersionHeaderProvider>();
            Mvx.IoCProvider.RegisterType<ConnectivityHeaderProvider>();
            Mvx.IoCProvider.RegisterType<MobileDownloadAllowedHeaderProvider>();
            Mvx.IoCProvider.RegisterType<HeaderRequestInterceptor>();
            Mvx.IoCProvider.RegisterType<IClaimUserInformationExtractor, ClaimUserInformationExtractor>();
            Mvx.IoCProvider.LazyConstructAndRegisterSingleton<ITrackPlayedStorage, TrackPlayedStorage>();
            InitializeApiClient(serverUri);
            Mvx.IoCProvider.LazyConstructAndRegisterSingleton<ICurrentUserLoader, CurrentUserLoader>();
            Mvx.IoCProvider.RegisterType<IStreamToFileSystemWriter, StreamToFileSystemWriter>();
            Mvx.IoCProvider.ConstructAndRegisterSingletonIfNotRegistered<IFileDownloader, HttpClientFileDownloader>();
            Mvx.IoCProvider.LazyConstructAndRegisterSingleton<IDownloadQueue, DownloadQueue>();

            Mvx.IoCProvider.LazyConstructAndRegisterSingleton<IOfflineTrackCollectionStorage, OfflineTrackCollectionStorage>();
            Mvx.IoCProvider.CallbackWhenRegistered<IOfflineTrackCollectionStorage>(storage => storage.InitAsync());
            Mvx.IoCProvider.LazyConstructAndRegisterSingleton<ITrackCollectionOfflineTrackProvider, TrackCollectionOfflineTrackProvider>();
            Mvx.IoCProvider.LazyConstructAndRegisterSingleton<IPodcastOfflineTrackProvider, PodcastOfflineTrackProvider>();

            Mvx.IoCProvider.RegisterType<IOfflinePlaylistStorage, OfflinePlaylistStorage>();
            Mvx.IoCProvider.RegisterType<IPlaylistOfflineTrackProvider, PlaylistOfflineTrackProvider>();
            Mvx.IoCProvider.RegisterType<IPlaylistManager, PlaylistManager>();

            Mvx.IoCProvider.RegisterType<IGlobalTrackProvider, GlobalTrackProvider>();
            Mvx.IoCProvider.RegisterType<IAppContentLogger, AppContentLogger>();
            Mvx.IoCProvider.RegisterType<ILanguagesLogger, LanguagesLogger>();
            Mvx.IoCProvider.ConstructAndRegisterSingleton<IGlobalMediaDownloader, GlobalMediaDownloader>();

            Mvx.IoCProvider.LazyConstructAndRegisterSingleton<IPodcastOfflineManager, PodcastOfflineManager>();
            Mvx.IoCProvider.CallbackWhenRegistered<IPodcastOfflineManager>(manager => manager.InitAsync());

            Mvx.IoCProvider.ConstructAndRegisterSingleton<IReceive<PodcastNotification>, PodcastNotificationReceiver>();
            Mvx.IoCProvider.RegisterType<IReceive<GeneralNotification>, GeneralNotificationReceiver>();
            Mvx.IoCProvider.RegisterType<INotificationHandler, NotificationHandler>();
            Mvx.IoCProvider.RegisterType<NotificationParser>();

            Mvx.IoCProvider.RegisterType<ILogoutService, LogoutService>();

            Mvx.IoCProvider.LazyConstructAndRegisterSingleton<ITrackCollectionManager, TrackCollectionManager>();

            Mvx.IoCProvider.LazyConstructAndRegisterSingleton<MediaFileUrlSetter, MediaFileUrlSetter>();

            Mvx.IoCProvider.RegisterType<IDocumentFilter, NullFilter>();
            Mvx.IoCProvider.RegisterType<IDownloadedTracksOnlyFilter, DownloadedTracksOnlyFilter>();

            Mvx.IoCProvider.RegisterType<IPlayerErrorHandler, PlayerErrorHandler>();
            Mvx.IoCProvider.LazyConstructAndRegisterSingleton<IPlayerAnalytics, PlayerAnalytics>();
            Mvx.IoCProvider.LazyConstructAndRegisterSingleton<IMediaPlayer, ViewModelHandlingMediaPlayerDecorator>();
            Mvx.IoCProvider.LazyConstructAndRegisterSingleton<ILiveTime, LiveTime>();
            Mvx.IoCProvider.RegisterType<IMeasurementCalculator, MeasurementCalculator>();
            Mvx.IoCProvider.RegisterDecorator<IPlayStatistics, LivestreamPlayStatisticsDecorator, PersistingPlayStatisticsDecorator, PlayStatistics>();
            Mvx.IoCProvider.ConstructAndRegisterSingleton<PlayObserverOrchestrator, PlayObserverOrchestrator>();
            Mvx.IoCProvider.ConstructAndRegisterSingleton<IReceiveLocal<WordOfFaithNotification>, AslaksenNotification>();
            Mvx.IoCProvider.RegisterType<IListenedTracksStorage, ListenedTracksStorage>();
            Mvx.IoCProvider.ConstructAndRegisterSingleton(typeof(TrackListenedObserver));
            Mvx.IoCProvider.ConstructAndRegisterSingleton<IStreakObserver, StreakObserver>();
            Mvx.IoCProvider.ConstructAndRegisterSingleton<ISubscriptionManager, SubscriptionManager>();

            Mvx.IoCProvider.IoCConstruct<ConnectionStatusLogger>();
            InitializeTranslation();
            RegisterCustomAppStart<AppStart>();

            Mvx.IoCProvider.Resolve<IMvxMessenger>()
                .Subscribe<LoggedOutMessage>(message =>
                    {
                        Mvx.IoCProvider.Resolve<IMediaPlayer>().Stop();

                        BlobCache.LocalMachine.InvalidateAll();
                        BlobCache.UserAccount.InvalidateAll();
                        BlobCache.Secure.InvalidateAll();
                        BlobCache.InMemory.InvalidateAll();
                    },
                    MvxReference.Strong);


            Mvx.IoCProvider.ConstructAndRegisterSingleton<BackgroundLogger, BackgroundLogger>();

            Mvx.IoCProvider.RegisterType<PersistedEventWriter>();
            Mvx.IoCProvider.RegisterType<FirebaseConfigUpdater>();
            Mvx.IoCProvider.RegisterType<AfterStartupSupportEndsPopupDisplayer>();
            Mvx.IoCProvider.RegisterType<PartialDownloadRemover>();
            Mvx.IoCProvider.RegisterType<PodcastInitializer>();
            Mvx.IoCProvider.RegisterType<OidcUserStartupTask>();

            Mvx.IoCProvider.ConstructAndRegisterSingleton<IStartupManager, StartupManager>();
            Mvx.IoCProvider.CallbackWhenRegistered<IStartupManager>(manager => manager.Initialize(CreatableTypes()));
            ImageService.Instance.Initialize(new FFImageLoading.Config.Configuration
            {
                HttpClient = new HttpClient(new AuthenticatedHttpImageClientHandler(Mvx.IoCProvider.Resolve<IMediaRequestHttpHeaders>()))
            });
        }

        private static void InitializeApiClient(ApiBaseUri serverUri)
        {
            var requestHandlerFactory = Mvx.IoCProvider.Resolve<IRequestHandlerFactory>();

            Mvx.IoCProvider.LazyConstructAndRegisterSingleton<IOidcAuthService, OidcAuthService>();

            Mvx.IoCProvider.LazyConstructAndRegisterSingleton<IUserAuthChecker, OidcAuthService>();

            Mvx.IoCProvider.RegisterSingleton(requestHandlerFactory.BuildRequestHandler());
            Mvx.IoCProvider.RegisterSingleton<ApiBaseUri>(serverUri);

            // Caching
            Mvx.IoCProvider.RegisterType<ICache, LocalStorageCache>();
            Mvx.IoCProvider.RegisterType<IClientCache, ClientCache>();

            // Different Clients
            Mvx.IoCProvider.RegisterType<IAlbumClient, AlbumClient>();
            Mvx.IoCProvider.RegisterType<IApiInfoClient, ApiInfoClient>();
            Mvx.IoCProvider.RegisterType<IFacetsClient, FacetsClient>();
            Mvx.IoCProvider.RegisterType<IFileClient, FileClient>();
            Mvx.IoCProvider.RegisterType<ILiveClient, LiveClient>();
            Mvx.IoCProvider.RegisterDecorator<IPodcastClient, CachedPodcastClientDecorator, PodcastClient>();
            Mvx.IoCProvider.RegisterDecorator<IPlaylistClient, CachedPlaylistClientDecorator, PlaylistClient>();
            Mvx.IoCProvider.RegisterType<ISearchClient, SearchClient>();
            Mvx.IoCProvider.LazyConstructAndRegisterSingleton<IStatisticsClient, OfflineStatisticsClient>();
            Mvx.IoCProvider.RegisterType<ISubscriptionClient, SubscriptionClient>();
            Mvx.IoCProvider.RegisterDecorator<ITrackCollectionClient, CachedTrackCollectionClientDecorator, TrackCollectionClient>();
            Mvx.IoCProvider.RegisterDecorator<ITracksClient, CachedTracksClientDecorator, TracksClient>();
            Mvx.IoCProvider.RegisterDecorator<IContributorClient, CachedContributorClientDecorator, ContributorClient>();
            Mvx.IoCProvider.RegisterType<IUsersClient, UsersClient>();
            Mvx.IoCProvider.RegisterDecorator<IDiscoverClient, CachedDiscoverClientDecorator, DiscoverClient>();

            Mvx.IoCProvider.LazyConstructAndRegisterSingleton<IBMMClient, InjectedBmmClient>();
        }

        private static void InitializeTranslation()
        {
            var provider = Mvx.IoCProvider.Resolve<IAppLanguageProvider>();
            var language = provider.GetAppLanguage();
            provider.InitializeAtStartup(language);
            TextProviderBuilder.SetDefaultLanguage(language);

            var builder = new TextProviderBuilder(new BmmJsonDictionaryTextProvider());
            Mvx.IoCProvider.RegisterSingleton<IMvxTextProviderBuilder>(builder);
            Mvx.IoCProvider.RegisterSingleton<IMvxTextProvider>(builder.TextProvider);
        }
    }
}
