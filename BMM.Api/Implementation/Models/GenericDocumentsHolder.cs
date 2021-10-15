using System.Collections.Generic;

namespace BMM.Api.Implementation.Models
{
    public class GenericDocumentsHolder : ITranslationDetailsHolder
    {
        public string Title { get; set; }
        public bool SupportsPaging { get; set; }
        public IEnumerable<Document> Items { get; set; }
        public string TranslationParent { get; set; }
        public string TranslationId { get; set; }
    }
}