using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace BMM.Api.Implementation.Models
{
    [JsonObject]
    public class TrackRaw : Document
    {
        public TrackRaw()
        {
            DocumentType = DocumentType.Track;
        }

        public string Comment { get; set; }

        public bool IsVisible { get; set; }

        public int Order { get; set; }

        public string OriginalLanguage { get; set; }

        public int ParentId { get; set; }

        public DateTime PublishedAt { get; set; }

        public DateTime RecordedAt { get; set; }

        [JsonProperty(PropertyName = "rel")]
        public IEnumerable<TrackRelation> Relations { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public TrackSubType Subtype { get; set; }

        public IEnumerable<string> Tags { get; set; }

        public IEnumerable<TrackTranslation> Translations { get; set; }
    }
}