using System.Collections.Generic;
using BMM.Api.Implementation.Models;

namespace BMM.Core.Models.PlaybackHistory.Interfaces
{
    public interface IPlaybackHistoryActionResult
    {
        IReadOnlyList<PlaybackHistoryEntry> PlaybackHistoryRawEntries { get; }
        IEnumerable<Document> Documents { get; }
    }
}