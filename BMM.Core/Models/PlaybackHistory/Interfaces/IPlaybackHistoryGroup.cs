using System;
using System.Collections.Generic;

namespace BMM.Core.Models.PlaybackHistory.Interfaces
{
    public interface IPlaybackHistoryGroup
    {
        IList<PlaybackHistoryEntry> PlayedTracks { get; }
        DateTime GroupDateTimeUTC { get; }
    }
}