using System.Collections.Generic;
using System.Linq;
using BMM.Api.Implementation.Models;
using BMM.Core.Models.PlaybackHistory.Interfaces;

namespace BMM.Core.Models.PlaybackHistory
{
    public class PlaybackHistoryActionResult : IPlaybackHistoryActionResult
    {
        public PlaybackHistoryActionResult(IReadOnlyList<PlaybackHistoryEntry> playbackHistoryRawEntries, IEnumerable<Document> documents)
        {
            PlaybackHistoryRawEntries = playbackHistoryRawEntries;
            Documents = documents;
        }

        public IReadOnlyList<PlaybackHistoryEntry> PlaybackHistoryRawEntries { get; }
        public IEnumerable<Document> Documents { get; }

        public static IPlaybackHistoryActionResult Empty()
        {
            return new PlaybackHistoryActionResult(
                Enumerable.Empty<PlaybackHistoryEntry>().ToList(),
                Enumerable.Empty<Document>());
        }
    }
}