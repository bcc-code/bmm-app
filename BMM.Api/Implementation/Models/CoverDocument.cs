using Newtonsoft.Json;

namespace BMM.Api.Implementation.Models
{
    [JsonObject]
    public class CoverDocument : Document
    {
        public string Cover { get; set; }
    }
}