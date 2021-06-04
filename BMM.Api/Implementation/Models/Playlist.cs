using Newtonsoft.Json;

namespace BMM.Api.Implementation.Models
{
    [JsonObject]
    public class Playlist : CoverDocument, ITrackListDisplayable
    {
        public Playlist()
        {

            DocumentType = DocumentType.Playlist;
        }

        public string Language { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }
    }
}
