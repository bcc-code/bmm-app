using System;
using BMM.Api.Implementation.Models;
using BMM.Core.Models.PlaybackHistory.Interfaces;

namespace BMM.Core.Models.PlaybackHistory
{
    public class PlaybackHistoryEntry : IPlaybackHistoryEntry
    {
        public PlaybackHistoryEntry(Track mediaTrack, long lastPosition, DateTime playedAtUtc)
        {
            MediaTrack = mediaTrack;
            LastPosition = lastPosition;
            PlayedAtUTC = playedAtUtc;
        }

        public Track MediaTrack { get; set; }
        public long LastPosition { get; set; }
        public DateTime PlayedAtUTC { get; }
    }
}