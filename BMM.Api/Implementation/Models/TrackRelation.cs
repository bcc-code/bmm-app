using BMM.Api.Framework.JsonConverter;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace BMM.Api.Implementation.Models
{
    [JsonConverter(typeof(TrackRelationConverter))]
    public class TrackRelation
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public TrackRelationType Type { get; set; }

        public bool HasListened { get; set; }
    }
}