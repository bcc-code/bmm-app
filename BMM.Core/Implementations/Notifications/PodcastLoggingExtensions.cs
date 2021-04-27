using System.Collections.Generic;
using BMM.Core.Implementations.Analytics;

namespace BMM.Core.Implementations.Notifications
{
    /// <summary>
    /// Temporary helper to log information about downloading the newest podcast.
    /// It's pretty hacky but that's ok since it can be removed once we fixed the bug.
    /// </summary>
    public static class PodcastLoggingExtensions
    {
        public static int? PodcastTrackIdToDownload;

        public static void LogForPodcast(this IAnalytics analytics, string message)
        {
            if (PodcastTrackIdToDownload.HasValue)
            {
                analytics.LogEvent("Podcast - " + message, new Dictionary<string, object> { { "TrackKey", PodcastTrackIdToDownload } });
            }
        }
    }
}
