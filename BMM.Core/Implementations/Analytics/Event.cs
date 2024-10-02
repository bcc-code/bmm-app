namespace BMM.Core.Implementations.Analytics
{
    public class Event
    {
        public const string PlayTrack = "Playback started";
        public const string TrackFinished = "Track finished";
        public const string QueueCompleted = "Queue completed";
        public const string TrackHasBeenAddedToEndOfQueue = "Track has been added to the end of the queue";
        public const string TrackHasBeenAddedToBePlayedNext = "Track has been added to be played next";
        public const string MediaBrowserConnectionFailed = "Media browser connection failed";
        public const string MediaBrowserConnectionSuspended = "Media browser connection suspended";
        public const string OptionsMenuHasBeenOpened = "Options menu has been opened";
        public const string TrackHasBeenDownloaded = "Track has been downloaded";
        public const string TrackDownloadingException = "Exception during track downloading";
        public const string PlaylistNotFoundException = "Playlist not found exception";
        public const string ViewModelOpenedFormat = "{0} opened";
        public const string EmptyPlayer = "Empty player";
        public const string AlbumResumed = "Album resumed";
        public const string LyricsOpened = "Lyrics opened";
        public const string TrackLanguageChanged = "Track language changed";
        public const string BackgroundActivityRestrictedPopupShown = "Background Activity restricted popup shown";
        public const string SleepTimerOptionsOpened = "Sleep timer options menu opened";
        public const string SleepTimerOptionSelected = "Sleep timer option selected";
        public const string PlaybackSpeedOptionsOpened = "Playback speed options menu opened";
        public const string PlaybackSpeedChanged = "Playback speed changed";
        public const string SiriSearch = "Siri search";
        public const string SiriMusicPlayed = "Siri music played";
        public const string SiriFromKaarePlayed = "Siri From Kåre played";
        public const string SiriDifferentLanguage = "Different Siri language";
        public const string BottomBarButtonClicked = "Bottom Bar button clicked";
        public const string YearInReviewShareClicked = "Year in review share clicked";
        public const string YearInReviewOpened = "Year in review opened";
        public const string YearInReviewPlaylistOpened = "Year in review playlist opened";
        public const string YearInReviewPlaylistAddedToFavorites = "Year in review playlist added to favorites";
        public const string ProblemWithOfflineFileDetected = "Problem with offline file detected";
        public const string NavigateToExternalLink = "Navigate to external link";
        public const string BCCMediaOpenedFromPlayer = "BCC Media opened from player";
        public const string AlbumAddedToTrackCollection = "added album to track_collection";
        public const string TrackAddedToTrackCollection = "added track to track_collection";
        public const string PlaylistAddedToTrackCollection = "added playlist to track_collection";
        public const string TrackCollectionAddedToTrackCollection = "added track collection to track_collection";
    }
}