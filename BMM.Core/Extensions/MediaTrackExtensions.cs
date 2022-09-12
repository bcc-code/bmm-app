using BMM.Api.Abstraction;

namespace BMM.Core.Extensions
{
    public static class MediaTrackExtensions
    {
        public static bool IsDownloaded(this IMediaTrack mediaTrack)
        {
            return !string.IsNullOrEmpty(mediaTrack.LocalPath);
        }
    }
}