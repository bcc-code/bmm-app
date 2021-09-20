using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BMM.Api.Implementation.Models;
using BMM.Core.GuardedActions.Base;
using BMM.Core.GuardedActions.PlaybackHistory.Interfaces;
using BMM.Core.Implementations.Player.Interfaces;
using BMM.Core.Models;

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
            var playbackHistoryGroups = await _playbackHistoryService.GetAll();
            var documents = new List<Document>();

            foreach (var playbackHistory in playbackHistoryGroups)
            {
                documents.Add(new ChapterHeader
                {
                    Title = playbackHistory.GroupDateTime.ToString(DateFormat)
                });

                var tracks = playbackHistory
                    .PlayedTracks
                    .Select(x => x.MediaTrack);

                documents.AddRange(tracks);
            }

            return documents;
        }
    }
}