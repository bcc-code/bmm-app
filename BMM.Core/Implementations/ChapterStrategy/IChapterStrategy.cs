using System.Collections.Generic;
using BMM.Api.Implementation.Models;

namespace BMM.Core.Implementations.ChapterStrategy
{
    public interface IChapterStrategy
    {
        IList<Document> AddChapterHeaders(IList<Track> tracks, IEnumerable<Document> existingDocs);
    }
}