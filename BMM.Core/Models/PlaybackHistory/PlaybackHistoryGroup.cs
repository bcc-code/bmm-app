using System;
using System.Collections.Generic;
using BMM.Core.Models.PlaybackHistory.Interfaces;

namespace BMM.Core.Models.PlaybackHistory
{
    public class PlaybackHistoryGroup : IPlaybackHistoryGroup
    {
        public PlaybackHistoryGroup(
            IList<PlaybackHistoryEntry> playedTracks,
            DateTime groupDateTime)
        {
            PlayedTracks = playedTracks;
            GroupDateTime = groupDateTime;
        }

        public IList<PlaybackHistoryEntry> PlayedTracks { get; }

        public DateTime GroupDateTime { get; }
    }
}