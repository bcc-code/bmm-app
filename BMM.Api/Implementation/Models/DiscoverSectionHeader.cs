using Newtonsoft.Json;

namespace BMM.Api.Implementation.Models
{
    [JsonObject]
    public class DiscoverSectionHeader : Document, ITranslationDetailsHolder
    {
        public DiscoverSectionHeader()
        {
            DocumentType = DocumentType.DiscoverSectionHeader;
        }

        public string Title { get; set; }

        public string Link { get; set; }

        public string TranslationParent { get; set; }

        public string TranslationId { get; set; }

        public bool UseCoverCarousel { get; set; }

        [JsonIgnore]
        public bool HasLink => string.IsNullOrEmpty(Link);

        [JsonIgnore]
        public bool IsSeparatorVisible { get; set; } = true;
    }
}