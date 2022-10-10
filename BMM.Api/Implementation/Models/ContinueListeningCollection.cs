using System.Collections.Generic;

namespace BMM.Api.Implementation.Models
{
    public class ContinueListeningCollection : Document
    {
        public ContinueListeningCollection(IEnumerable<Document> continueListeningElements)
        {
            ContinueListeningElements = continueListeningElements;
            DocumentType = DocumentType.ContinueListeningCollection;
        }

        public IEnumerable<Document> ContinueListeningElements { get; }
    }
}