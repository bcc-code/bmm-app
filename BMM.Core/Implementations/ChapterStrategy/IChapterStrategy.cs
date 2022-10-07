using System.Collections.Generic;
using BMM.Api.Implementation.Models;
using BMM.Core.Models.POs.Base;
using BMM.Core.Models.POs.Base.Interfaces;

namespace BMM.Core.Implementations.ChapterStrategy
{
    public interface IChapterStrategy
    {
        IList<Document> AddChapterHeaders(IList<Track> tracks, IEnumerable<IDocumentPO> existingDocs);
    }
}