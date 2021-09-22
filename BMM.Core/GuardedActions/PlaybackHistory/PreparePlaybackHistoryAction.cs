using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BMM.Api.Implementation.Models;
using BMM.Core.GuardedActions.Base;
using BMM.Core.GuardedActions.PlaybackHistory.Interfaces;
using BMM.Core.Implementations.Localization.Interfaces;
using BMM.Core.Implementations.Player.Interfaces;
using BMM.Core.Models;
using BMM.Core.Models.PlaybackHistory;
using BMM.Core.Models.PlaybackHistory.Interfaces;
using BMM.Core.Translation;

namespace BMM.Core.GuardedActions.PlaybackHistory
{
    public class PreparePlaybackHistoryAction
        : GuardedActionWithResult<IEnumerable<Document>>,
          IPreparePlaybackHistoryAction
    {
        private const string DateFormat = "dddd, dd MMMM";
        private readonly IPlaybackHistoryService _playbackHistoryService;
        private readonly IBMMLanguageBinder _bmmLanguageBinder;

        public PreparePlaybackHistoryAction(
            IPlaybackHistoryService playbackHistoryService,
            IBMMLanguageBinder bmmLanguageBinder)
        {
            _playbackHistoryService = playbackHistoryService;
            _bmmLanguageBinder = bmmLanguageBinder;
        }

        protected override async Task<IEnumerable<Document>> Execute()
        {
            var playbackHistoryEntries = await _playbackHistoryService.GetAll();

            if (!playbackHistoryEntries.Any())
                return Enumerable.Empty<Document>();

            var documents = new List<Document>();

            var groupedEntries = GroupByDay(playbackHistoryEntries);

            foreach (var groupedEntry in groupedEntries)
            {
                documents.Add(new ChapterHeader
                {
                    Title = GetTitle(groupedEntry)
                });

                var tracks = groupedEntry
                    .PlayedTracks
                    .Select(t => t.MediaTrack)
                    .ToList();

                documents.AddRange(tracks);
            }

            return documents;
        }

        private static IEnumerable<PlaybackHistoryGroup> GroupByDay(IEnumerable<PlaybackHistoryEntry> playbackHistoryEntries)
        {
            return playbackHistoryEntries
                .OrderByDescending(x => x.PlayedAtUTC.ToLocalTime())
                .GroupBy(l => new
                {
                    l.PlayedAtUTC.ToLocalTime().Year,
                    l.PlayedAtUTC.ToLocalTime().Month,
                    l.PlayedAtUTC.ToLocalTime().Day
                })
                .Where(g => g.Any())
                .Select(g => new PlaybackHistoryGroup(g.ToList(), g.First().PlayedAtUTC.ToLocalTime()))
                .ToList();
        }

        private string GetTitle(IPlaybackHistoryGroup groupedEntry)
        {
            if (groupedEntry.GroupDateTime.Date == DateTime.Today)
                return _bmmLanguageBinder[Translations.Global_Today];

            if (groupedEntry.GroupDateTime.Date == DateTime.Today.AddDays(-1))
                return _bmmLanguageBinder[Translations.Global_Yesterday];

            return groupedEntry.GroupDateTime.ToString(DateFormat);
        }
    }
}