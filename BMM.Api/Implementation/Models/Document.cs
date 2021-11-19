using BMM.Api.Framework.JsonConverter;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace BMM.Api.Implementation.Models
{
    [JsonObject, JsonConverter(typeof(DocumentConverter))]
    public abstract class Document
    {
        [JsonProperty(PropertyName = "type"), JsonConverter(typeof(StringEnumConverter))]
        public DocumentType DocumentType { get; set; }

        public int Id { get; set; }
    }
}