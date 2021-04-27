using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace BMM.Api.Implementation.Models
{
    public class TrackMedia
    {
        public IEnumerable<TrackMediaFile> Files { get; set; }

        public bool IsVisible { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public TrackMediaType Type { get; set; }
    }
}