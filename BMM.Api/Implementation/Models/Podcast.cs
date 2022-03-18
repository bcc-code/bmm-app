using BMM.Api.Implementation.Models.Interfaces;
using Newtonsoft.Json;

namespace BMM.Api.Implementation.Models
{
    [JsonObject]
    public class Podcast : CoverDocument, ITrackListDisplayable
    {
        public Podcast()
        {
            DocumentType = DocumentType.Podcast;
        }

        public string Language { get; set; }

        public string Title { get; set; }
    }
}