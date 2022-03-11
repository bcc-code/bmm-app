using System.Collections.Generic;

namespace BMM.Api.Implementation.Models
{
    public class SearchResults
    {
        public IEnumerable<Document> Items { get; set; }
        /// <summary>
        /// The server counts documents differently. Therefore we can't just count how many docs we received.
        /// Instead we need to rely on this field to get the from position of the next page.
        /// </summary>
        public int NextPageFromPosition { get; set; }
    }
}