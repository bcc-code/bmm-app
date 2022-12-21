using System.Collections.Generic;

namespace BMM.Api.Implementation.Models
{
    public class TilesCollection : Document
    {
        public TilesCollection(IEnumerable<Document> tileElements)
        {
            TileElements = tileElements;
            DocumentType = DocumentType.TileCollection;
        }

        public IEnumerable<Document> TileElements { get; }
    }
}