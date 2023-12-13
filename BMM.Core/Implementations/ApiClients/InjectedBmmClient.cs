using BMM.Api;
using BMM.Api.Implementation.Clients.Contracts;
using MvvmCross;

namespace BMM.Core.Implementations.ApiClients
{
    public class InjectedBmmClient : IBMMClient
    {
        public IAlbumClient Albums { get; }

        public IApiInfoClient ApiInfo { get; }

        public IContributorClient Contributors { get; }

        public IFacetsClient Facets { get; }

        public IFileClient File { get; }

        public IPodcastClient Podcast { get; }

        public IPlaylistClient Playlist { get; }

        public ISearchClient Search { get; }

        public IStatisticsClient Statistics { get; }

        public ISubscriptionClient Subscription { get; }

        public ITrackCollectionClient TrackCollection { get; }

        public ITracksClient Tracks { get; }

        public IUsersClient Users { get; }

        public IDiscoverClient Discover { get; }

        public ISharedPlaylistClient SharedPlaylist { get; }

        public IBrowseClient Browse { get; }
        
        public IQuestionsClient QuestionsClient { get; }

        public InjectedBmmClient()
        {
            ApiInfo = Mvx.IoCProvider.Resolve<IApiInfoClient>();
            Users = Mvx.IoCProvider.Resolve<IUsersClient>();
            Tracks = Mvx.IoCProvider.Resolve<ITracksClient>();
            Albums = Mvx.IoCProvider.Resolve<IAlbumClient>();
            Contributors = Mvx.IoCProvider.Resolve<IContributorClient>();
            Search = Mvx.IoCProvider.Resolve<ISearchClient>();
            TrackCollection = Mvx.IoCProvider.Resolve<ITrackCollectionClient>();
            Podcast = Mvx.IoCProvider.Resolve<IPodcastClient>();
            Playlist = Mvx.IoCProvider.Resolve<IPlaylistClient>();
            Statistics = Mvx.IoCProvider.Resolve<IStatisticsClient>();
            Subscription = Mvx.IoCProvider.Resolve<ISubscriptionClient>();
            Facets = Mvx.IoCProvider.Resolve<IFacetsClient>();
            File = Mvx.IoCProvider.Resolve<IFileClient>();
            Discover = Mvx.IoCProvider.Resolve<IDiscoverClient>();
            SharedPlaylist = Mvx.IoCProvider.Resolve<ISharedPlaylistClient>();
            Browse = Mvx.IoCProvider.Resolve<IBrowseClient>();
            QuestionsClient = Mvx.IoCProvider.Resolve<IQuestionsClient>();
        }
    }
}