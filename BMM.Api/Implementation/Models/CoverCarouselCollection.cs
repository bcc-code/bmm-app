using System.Collections.Generic;

namespace BMM.Api.Implementation.Models
{
    public class CoverCarouselCollection : Document
    {
        public CoverCarouselCollection(IEnumerable<Document> coverDocuments)
        {
            CoverDocuments = coverDocuments;
            DocumentType = DocumentType.PlaylistsCollection;
        }

        public IEnumerable<Document> CoverDocuments { get; }
    }
}