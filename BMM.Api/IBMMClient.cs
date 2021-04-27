using BMM.Api.Implementation.Clients.Contracts;

namespace BMM.Api
{
    public interface IBMMClient
    {
        IAlbumClient Albums { get; }

        IApiInfoClient ApiInfo { get; }

        IContributorClient Contributors { get; }

        IFacetsClient Facets { get; }

        IFileClient File { get; }

        IPodcastClient Podcast { get; }

        IPlaylistClient Playlist { get; }

        ISearchClient Search { get; }

        IStatisticsClient Statistics { get; }

        ISubscriptionClient Subscription { get; }

        ITrackCollectionClient TrackCollection { get; }

        ITracksClient Tracks { get; }

        IUsersClient Users { get; }

        IDiscoverClient Discover { get; }
    }
}