using System;
using System.Collections.Generic;
using BMM.Core.Models.PlaybackHistory.Interfaces;

namespace BMM.Core.Models.PlaybackHistory
{
    public class PlaybackHistoryGroup : IPlaybackHistoryGroup
    {
        public PlaybackHistoryGroup(
            IList<PlaybackHistoryEntry> playedTracks,
            DateTime groupDateTimeUTC)
        {
            PlayedTracks = playedTracks;
            GroupDateTimeUTC = groupDateTimeUTC;
        }

        public IList<PlaybackHistoryEntry> PlayedTracks { get; }

        public DateTime GroupDateTimeUTC { get; }
    }
}