namespace BMM.Api.Framework.HTTP
{
    public class ApiUris
    {
        public static string ApiRoot = "";
        public static string Albums = "album/{?tags[]*,content%2Dtype[]*,language,size,from}";
        public static string AlbumsByPublishedYear = "album/published/{year}/";
        public static string AlbumsByRecordedYear = "album/tracks_recorded/{year}/";
        public static string Album = "album/{id}";
        public static string AlbumCover = "album/{id}/cover";
        public static string Contributors = "contributor/{?orderby,size,from}";
        public static string ContributorsByName = "contributor/suggester/completion/{term}{?size}";
        public static string Contributor = "contributor/{id}";
        public static string ContributorCover = "contributor/{id}/cover";
        public static string ContributorTracks = "contributor/{id}/track/{?size,from,content%2Dtype[]*,language}";
        public static string ContributorRandomTracks = "contributor/{id}/random/{?size}";
        public static string Login = "login/authentication";
        public static string LoginJwt = "login/authentication_jwt";
		public static string Notifications = "notifications/{deviceId}";
        public static string Live = "live";
        public static string CurrentUser = "CurrentUser";

        public static string Search = "search/v2/{term}{?size,from}";

        public static string Suggestions = "suggest/{term}";
        public static string Tracks = "track/{?tags[]*,exclude%2Dtags[]*,content%2Dtype[]*,language,size,from}";
        public static string TracksRelated = "track/rel/{key}/{?content%2Dtype[]*,size,from}";
        public static string Track = "track/{id}{?raw}";
        public static string TrackRecommendation = "track/recommendation";
        public static string TrackCover = "track/{id}/cover";
        public static string TrackCollections = "track_collection/";
        public static string TrackCollection = "track_collection/{id}";
        public static string TrackCollectionAlbum = "track_collection/{id}/album/{albumId}";
        public static string TrackCollectionResetShare = "track_collection/{id}/reset-share";
        public static string TrackCollectionUnfollow = "track_collection/{id}/unfollow";
        public static string TrackCollectionTopSongs = "track_collection/top-songs";
        public static string TrackFiles = "track/{id}/files/";
        public static string StatisticsGlobalDownloadedMost = "statistics/global/{type}/downloaded/most{?size,from}";
        public const string StatisticsPostTrackPlayedEvent = "statistics/track/played/";

        public const string Podcasts = "podcast/";
        public const string Podcast = "podcast/{id}";
        public const string PodcastTracks = "podcast/{id}/track/{?size,from}";
        public const string PodcastCover = "podcast/{id}/cover/";
        public const string PodcastRandom = "podcast/{id}/random";
        public const string Shuffle = "podcast/{id}/shuffle{?size}";

        public const string Playlists = "playlist";
        public const string Playlist = "playlist/{id}";
        public const string PlaylistTracks = "playlist/{id}/track";
        public const string PlaylistDocuments = "playlist/documents{?lang,age}";
        public const string PlaylistCover = "podcast/{id}/cover";

        public static string FacetsAlbumPublishedYears = "facets/album_published/years";

        public static string StatisticsGlobalDownloadedRecently =
            "statistics/global/{type}/downloaded/recently{?size,from}";

        public static string StatisticsGlobalViewedMost = "statistics/global/{type}/viewed/most{?size,from}";
        public static string StatisticsGlobalViewedRecently = "statistics/global/{type}/viewed/recently{?size,from}";

        public static string StatisticsUserDownloadedMost =
            "statistics/user/{username}/{type}/downloaded/most{?size,from}";

        public static string StatisticsUserDownloadedRecently =
            "statistics/user/{username}/{type}/downloaded/recently{?size,from}";

        public static string StatisticsUserViewedMost = "statistics/user/{username}/{type}/viewed/most{?size,from}";

        public static string StatisticsUserViewedRecently =
            "statistics/user/{username}/{type}/viewed/recently{?size,from}";

        public static string YearInReview =
            "statistics/year-in-review/overview";
        
        public static string Discover = "discover{?lang,age}";

        public static string SharedPlaylist = "/shared_playlist/{sharingSecret}";
        public static string SharedPlaylistFollow = "/shared_playlist/{sharingSecret}/follow";

        public static string Browse = "browse";
        public static string BrowseEvents = "browse/events{?skip,take}";
    }
}