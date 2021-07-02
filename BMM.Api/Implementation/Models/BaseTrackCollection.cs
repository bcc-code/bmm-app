using System.Collections.Generic;
using Newtonsoft.Json;

namespace BMM.Api.Implementation.Models
{
    [JsonObject]
    public class BaseTrackCollection : Document
    {
        public BaseTrackCollection()
        {
            DocumentType = DocumentType.TrackCollection;
        }

        public string Name { get; set; }
    }
}