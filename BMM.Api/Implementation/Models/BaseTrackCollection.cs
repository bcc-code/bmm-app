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

        private IList<string> _access;

        public IList<string> Access { get => _access ?? (_access = new List<string>()); set => _access = value; }

        public string Name { get; set; }
    }
}