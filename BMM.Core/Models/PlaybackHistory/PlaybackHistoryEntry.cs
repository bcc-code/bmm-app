using System;
using BMM.Api.Implementation.Models;
using BMM.Core.Models.PlaybackHistory.Interfaces;

namespace BMM.Core.Models.PlaybackHistory
{
    public class PlaybackHistoryEntry : IPlaybackHistoryEntry
    {
        public PlaybackHistoryEntry(Track mediaTrack, DateTime playedAtUtc)
        {
            MediaTrack = mediaTrack;
            PlayedAtUTC = playedAtUtc;
        }

        public Track MediaTrack { get; }
        public DateTime PlayedAtUTC { get; }
    }
}