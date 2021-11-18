using Newtonsoft.Json;

namespace BMM.Api.Implementation.Models
{
    [JsonObject]
    public abstract class CoverDocument : Document
    {
        public string Cover { get; set; }
    }
}