namespace BMM.Core.Helpers
{
    public class StorageKeys
    {
        public static readonly string CurrentUser = "user";
        public static readonly string LanguageApp = "language_app";
        public static readonly string LanguageContent = "language_content";
        public static readonly string History = "history";

        public static readonly string MobileDownloadEnabled = "mobile_download_enabled";
        public static readonly string AutoplayEnabled = "autoplay_enabled";
        public static readonly string StreakHidden = "streak_hidden";
        public static readonly string PushNotificationsEnabled = "push_notifications_enabled";
        public static readonly string UseExternalStorage = "use_external_storage";

        public static readonly string LocalPodcasts = "local_podcasts";
        public static readonly string AutomaticallyDownloadedTracks = "local_podcasts_automatic_download";
        public static readonly string LocalTrackCollections = "local_track_collections";

        public const string UnfinishedTrackPlayedEvent = "play_statistics";
        public const string FinishedTrackPlayedEvents = "finished_track_played_events";
        public static readonly string CurrentDownload = "current_download";
        public const string LatestListeningStreak = "latest_listening_streak";

        public const string ListenedTracks = "listened_tracks";
        public const string NotifiedAslaksenTracks = "notified_aslaksen_tracks";
        public const string LastAslaksenNotification = "last_aslaksen_notification";

        public const string LocalPlaylists = "local_playlists";
    }
}