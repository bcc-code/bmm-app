using System;
using BMM.Api.Implementation.Models;

namespace BMM.Core.Implementations.PlayObserver.Model
{
    public class PlayMeasurements
    {
        /// <summary>
        /// The duration of a track is stored in the database as seconds and converted in <see cref="Api.Implementation.Models.Track.Duration"/> into ms just by multiplying it by 1000.
        /// This actually means the duration is already Ceiling'ed so that's why we also use the same approximation for msListened here.
        /// </summary>
        public double UniqueSecondsListened { get; set; }
        public ListenedStatus Status { get; set; }
        public double Percentage { get; set; }
        /// <summary>
        /// Length of the Track in Seconds
        /// </summary>
        public double TrackLength { get; set; }

        public DateTime TimestampStart { get; set; }
        public DateTime TimestampEnd { get; set; }

        public double SpentTime { get; set; }

        public long LastPosition { get; set; }
        
        public decimal AdjustedPlaybackSpeed { get; set; }
    }
}