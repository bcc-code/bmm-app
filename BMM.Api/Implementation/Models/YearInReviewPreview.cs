using Newtonsoft.Json;

namespace BMM.Api.Implementation.Models
{
    [JsonObject]
    public class YearInReviewPreview : Document
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string ButtonTitle { get; set; }
        public string ButtonLink { get; set; }
        public string PlaylistName { get; set; }
    }
}