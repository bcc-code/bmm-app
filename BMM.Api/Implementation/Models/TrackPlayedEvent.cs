using System;
using System.Collections.Generic;

namespace BMM.Api.Implementation.Models
{
    public class TrackPlayedEvent
    {
        public Guid Id { get; set; }

        public int? PersonId { get; set; }

        public string AnalyticsId { get; set; }

        public int? TrackId { get; set; }

        public double? UniqueSecondsListened { get; set; }

        public ListenedStatus? Status { get; set; }

        public double? Percentage { get; set; }

        /// <summary>
        /// Length of the Track in Seconds
        /// </summary>
        public double? TrackLength { get; set; }

        public DateTime? TimestampStart { get; set; }

        public DateTime? TimestampEnd { get; set; }

        public double? SpentTime { get; set; }

        public TrackSubType? TypeOfTrack { get; set; }

        public ResourceAvailability? Availability { get; set; }

        public int? AlbumId { get; set; }

        public IEnumerable<string> Tags { get; set; }

        public bool? SentAfterStartup { get; set; }

        public string Language { get; set; }

        public string PlaybackOrigin { get; set; }

        public long LastPosition { get; set; }

        public Track Track { get; set; }

        private sealed class IdEqualityComparer : IEqualityComparer<TrackPlayedEvent>
        {
            public bool Equals(TrackPlayedEvent x, TrackPlayedEvent y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;
                return x.Id.Equals(y.Id);
            }

            public int GetHashCode(TrackPlayedEvent obj)
            {
                return obj.Id.GetHashCode();
            }
        }

        public static IEqualityComparer<TrackPlayedEvent> IdComparer { get; } = new IdEqualityComparer();
    }
}