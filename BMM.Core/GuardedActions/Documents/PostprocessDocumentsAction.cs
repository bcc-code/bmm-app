using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BMM.Api.Implementation.Models;
using BMM.Core.GuardedActions.Base;
using BMM.Core.GuardedActions.Documents.Interfaces;
using BMM.Core.Implementations.TrackListenedObservation;
using MvvmCross;

namespace BMM.Core.GuardedActions.Documents
{
    public class PostprocessDocumentsAction : GuardedActionWithParameterAndResult<IEnumerable<Document>, IEnumerable<Document>>, IPostprocessDocumentsAction
    {
        private readonly IListenedTracksStorage _listenedTracksStorage;

        public PostprocessDocumentsAction(IListenedTracksStorage listenedTracksStorage)
        {
            _listenedTracksStorage = listenedTracksStorage;
        }

        protected override async Task<IEnumerable<Document>> Execute(IEnumerable<Document> documents)
        {
            if (documents == null)
                return null;

            var docList = documents.ToList();

            var videoDocuments = docList
                .OfType<Track>()
                .Where(t => t.Subtype == TrackSubType.Video);

            var filteredDocuments = docList
                .Where(d => videoDocuments.All(v => v.Id != d.Id))
                .ToList();

            var listenedTracksStorage = Mvx.IoCProvider.Resolve<IListenedTracksStorage>();
            foreach (var doc in filteredDocuments)
            {
                if (doc is Track track)
                    track.IsListened = await listenedTracksStorage.TrackIsListened(track);
            }

            return filteredDocuments;
        }
    }
}