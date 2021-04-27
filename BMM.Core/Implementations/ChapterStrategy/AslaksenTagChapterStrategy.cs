using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using BMM.Api.Implementation.Models;
using BMM.Core.Helpers;
using BMM.Core.Models;
using BMM.Core.ViewModels;
using MvvmCross.Localization;

namespace BMM.Core.Implementations.ChapterStrategy
{
    public class AslaksenTagChapterStrategy : IChapterStrategy
    {
        private readonly MvxLanguageBinder _textSource = new MvxLanguageBinder(GlobalConstants.GeneralNamespace, nameof(PodcastViewModel));

        public IList<Document> AddChapterHeaders(IList<Track> tracks, IEnumerable<Document> existingDocs)
        {
            var chapters = existingDocs.OfType<ChapterHeader>().ToList();
            var aslaksenTextRegex = new Regex("aslaksen - \\d");
            var chapterIdRegex = new Regex("\\d");
            var groupedTracks = tracks.GroupBy(t => t.Tags.First(tag => aslaksenTextRegex.IsMatch(tag)));

            var trackWithChapters = new List<Document>();
            foreach (var groupedTrack in groupedTracks)
            {
                var tracksForTag = groupedTrack.OfType<Document>().ToList();
                var match = chapterIdRegex.Match(groupedTrack.Key);
                if (!ChapterForTagExists(int.Parse(match.Value), chapters))
                {
                    var chapter = new ChapterHeader
                    {
                        Id = int.Parse(match.Value),
                        Title = _textSource.GetText("AslaksenTheme" + match.Value)
                    };
                    tracksForTag.Insert(0, chapter);
                }

                trackWithChapters.AddRange(tracksForTag);
            }

            return trackWithChapters;
        }

        private bool ChapterForTagExists(int tagId, IEnumerable<ChapterHeader> chapters)
        {
            return chapters.Any(c => c.Id == tagId);
        }
    }
}