using System.Collections.ObjectModel;

namespace BMM.Api.Implementation.Models
{
    public class CoverCarouselCollection : Document
    {
        public CoverCarouselCollection(ObservableCollection<Document> coverDocuments)
        {
            CoverDocuments = coverDocuments;
            DocumentType = DocumentType.PlaylistsCollection;
        }

        public ObservableCollection<Document> CoverDocuments { get; }
    }
}