using Newtonsoft.Json;

namespace BMM.Api.Implementation.Models
{
    [JsonObject]
    public class YearInReview
    {
        public string Url { get; set; }
        public string Subtitle { get; set; }
        public string Description { get; set; }
        public string Color { get; set; }
    }
}