using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BMM.Api.Implementation.Models;
using BMM.Core.GuardedActions.Base;
using BMM.Core.GuardedActions.Documents.Interfaces;
using BMM.Core.Implementations.TrackListenedObservation;
using BMM.Core.Models.POs.Base;
using BMM.Core.Models.POs.Base.Interfaces;
using BMM.Core.Models.POs.Tracks;
using BMM.Core.Models.POs.Tracks.Interfaces;

namespace BMM.Core.GuardedActions.Documents
{
    public class PostprocessDocumentsAction : GuardedActionWithParameterAndResult<IEnumerable<IDocumentPO>, IEnumerable<IDocumentPO>>, IPostprocessDocumentsAction
    {
        protected override async Task<IEnumerable<IDocumentPO>> Execute(IEnumerable<IDocumentPO> documents)
        {
            if (documents == null)
                return null;

            var docList = documents.ToList();

            var videoDocuments = docList
                .OfType<ITrackPO>()
                .Where(t => t.Track.Subtype == TrackSubType.Video);

            var filteredDocuments = docList
                .Where(d => videoDocuments.All(v => v.Id != d.Id))
                .ToList();

            return filteredDocuments;
        }
    }
}