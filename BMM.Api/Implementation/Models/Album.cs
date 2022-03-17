using System;
using System.Collections.Generic;
using BMM.Api.Implementation.Models.Interfaces;
using Newtonsoft.Json;

namespace BMM.Api.Implementation.Models
{
    [JsonObject]
    public class Album : CoverDocument, ITrackListDisplayable
    {
        public Album()
        {
            DocumentType = DocumentType.Album;
            Children = new List<Document>();
        }

        public string BmmId { get; set; }

        public IList<Document> Children { get; set; }

        public string Language { get; set; }

        public IEnumerable<string> Languages { get; set; }

        [JsonProperty(PropertyName = "_meta")]
        public AlbumMetadata Meta { get; set; }

        public int? ParentId { get; set; }

        public DateTime PublishedAt { get; set; }

        public IEnumerable<string> Tags { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }
        
        public int? LatestTrackId { get; set; }
        public int LatestTrackPosition { get; set; }
    }
}