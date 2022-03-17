using System.Collections.ObjectModel;

namespace BMM.Api.Implementation.Models
{
    public class ContinueListeningCollection : Document
    {
        public ContinueListeningCollection(ObservableCollection<Document> continueListeningElements)
        {
            ContinueListeningElements = continueListeningElements;
            DocumentType = DocumentType.ContinueListeningCollection;
        }

        public ObservableCollection<Document> ContinueListeningElements { get; }
    }
}