using System.Collections.Generic;

namespace BMM.Api.Implementation.Models
{
    public class GenericDocumentsHolder
    {
        public string Title { get; set; }
        public string TranslationString { get; set; }
        public bool SupportsPaging { get; set; }
        public IEnumerable<Document> Items { get; set; }
    }
}