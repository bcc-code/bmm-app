using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BMM.Api.Implementation.Models;
using BMM.Core.GuardedActions.Base;
using BMM.Core.GuardedActions.PlaybackHistory.Interfaces;
using BMM.Core.Implementations.Player.Interfaces;
using BMM.Core.Models;
using BMM.Core.Models.PlaybackHistory;

namespace BMM.Core.GuardedActions.PlaybackHistory
{
    public class PreparePlaybackHistoryAction
        : GuardedActionWithResult<IEnumerable<Document>>,
          IPreparePlaybackHistoryAction
    {
        private const string DateFormat = "dddd, dd MMMM";
        private readonly IPlaybackHistoryService _playbackHistoryService;

        public PreparePlaybackHistoryAction(IPlaybackHistoryService playbackHistoryService)
        {
            _playbackHistoryService = playbackHistoryService;
        }

        protected override async Task<IEnumerable<Document>> Execute()
        {
            var playbackHistoryEntries = await _playbackHistoryService.GetAll();

            if (!playbackHistoryEntries.Any())
                return Enumerable.Empty<Document>();

            var documents = new List<Document>();

            var groupedEntries = playbackHistoryEntries
                .OrderByDescending(x => x.PlayedAtUTC)
                .GroupBy(l => new
                {
                    l.PlayedAtUTC.Year,
                    l.PlayedAtUTC.Month,
                    l.PlayedAtUTC.Day
                })
                .Where(g => g.Any())
                .Select(g => new PlaybackHistoryGroup(g.ToList(), g.First().PlayedAtUTC))
                .ToList();

            foreach (var groupedEntry in groupedEntries)
            {
                documents.Add(new ChapterHeader
                {
                    Title = groupedEntry.GroupDateTime.ToString(DateFormat)
                });

                var tracks = groupedEntry
                    .PlayedTracks
                    .Select(x => x.MediaTrack);

                documents.AddRange(tracks);
            }

            return documents;
        }
    }
}