using Newtonsoft.Json;

namespace BMM.Api.Implementation.Models
{
    [JsonObject]
    public abstract class BaseTrackCollection : Document
    {
        protected BaseTrackCollection()
        {
            DocumentType = DocumentType.TrackCollection;
        }

        public string Name { get; set; }
    }
}