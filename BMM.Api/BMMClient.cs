using BMM.Api.Framework;
using BMM.Api.Framework.HTTP;
using BMM.Api.Implementation;
using BMM.Api.Implementation.Clients;
using BMM.Api.Implementation.Clients.Contracts;

namespace BMM.Api
{
    public class BMMClient : IBMMClient
    {
        // CONSTRUCTORS
        public BMMClient(ApiBaseUri baseUri,
            IRequestInterceptor requestInterceptor,
            IRequestHandlerFactory requestHandlerFactory,
            ILogger logger)
        {
            InitClients(requestHandlerFactory.CreateInstance(requestInterceptor), baseUri, logger);
        }

        public IAlbumClient Albums { get; private set; }

        // CLIENTS
        public IApiInfoClient ApiInfo { get; private set; }

        // PUBLIC GETTERS AND SETTERS
        public IContributorClient Contributors { get; private set; }

        public IFacetsClient Facets { get; private set; }

        public IFileClient File { get; private set; }

        public IPodcastClient Podcast { get; private set; }

        public IPlaylistClient Playlist { get; private set; }

        public ISearchClient Search { get; private set; }

        public IStatisticsClient Statistics { get; private set; }

        public ISubscriptionClient Subscription { get; private set; }

        public ITrackCollectionClient TrackCollection { get; private set; }

        public ITracksClient Tracks { get; private set; }

        public IUsersClient Users { get; private set; }

        public IDiscoverClient Discover { get; private set; }

        private void InitClients(IRequestHandler requestHandler, ApiBaseUri uri, ILogger logger)
        {
            ApiInfo = new ApiInfoClient(requestHandler, uri, logger);
            Users = new UsersClient(requestHandler, uri, logger);
            Tracks = new TracksClient(requestHandler, uri, logger);
            Albums = new AlbumClient(requestHandler, uri, logger);
            Contributors = new ContributorClient(requestHandler, uri, logger);
            Search = new SearchClient(requestHandler, uri, logger);
            TrackCollection = new TrackCollectionClient(requestHandler, uri, logger);
            Podcast = new PodcastClient(requestHandler, uri, logger);
            Playlist = new PlaylistClient(requestHandler, uri, logger);
            Statistics = new StatisticsClient(requestHandler, uri, logger);
            Subscription = new SubscriptionClient(requestHandler, uri, logger);
            Facets = new FacetsClient(requestHandler, uri, logger);
            File = new FileClient(requestHandler, uri, logger);
            Discover = new DiscoverClient(requestHandler, uri, logger);
        }
    }
}