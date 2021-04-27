using System.Collections.Generic;

namespace BMM.Core.Implementations.Notifications.Data
{
    public class PodcastNotification: RemoteNotification
    {
        public const string PodcastIdKey = "podcast_id";
        public const string TrackIdsKey = "track_ids";

        public const string Type = "podcast_notification";

        public int PodcastId { get; }
        public IEnumerable<int> TrackIds { get; }
        public bool IsDownloaded { get; set; }

        public PodcastNotification(int podcastId, IEnumerable<int> trackIds)
        {
            PodcastId = podcastId;
            TrackIds = trackIds;
            IsDownloaded = false;
        }
    }
}