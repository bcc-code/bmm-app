using BMM.Api.Abstraction;

namespace BMM.Core.Implementations.Analytics
{
    public interface IPlayerAnalytics
    {
        void TrackPlaybackRequested(ITrackModel track);
        void TrackPlaybackStarted(ITrackModel track);
        void MediaBrowserConnectionFailed();
        void MediaBrowserConnectionSuspended();
        void LogIfDownloadedTrackHasDifferentAttributesThanTrackFromTheApi(IMediaTrack downloadedTrack);
    }
}
