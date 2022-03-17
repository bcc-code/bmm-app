using BMM.Api.Implementation.Models.Interfaces;

namespace BMM.Api.Implementation.Models
{
    public class InfoMessage : Document, ITranslationDetailsHolder
    {
        public string TranslatedMessage { get; set; }
        public string TranslationParent { get; set; }
        public string TranslationId { get; set; }
        public string MessageText { get; set; }
    }
}