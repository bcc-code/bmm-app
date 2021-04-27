using System;
using BMM.Api.Abstraction;
using Newtonsoft.Json;

namespace BMM.Api.Implementation.Models
{
    public class TrackMetadata : ITrackMetadata
    {
        public string Album { get; set; }

        public string Artist { get; set; }

        public string AttachedPicture { get; set; }

        public string Composer { get; set; }

        public string Copyright { get; set; }

        public string Date { get; set; }

        public bool IsVisible { get; set; }

        public int Itunescompilation { get; set; }

        public string Language { get; set; }

        public string Lyricist { get; set; }

        public DateTime ModifiedAt { get; set; }

        public string ModifiedBy { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Album Parent { get; set; }

        public string Publisher { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Album RootParent { get; set; }

        public int RootParentId { get; set; }

        public string Time { get; set; }

        // ID3 Data ...

        public string Title { get; set; }

        public int Tracknumber { get; set; }

        public string Year { get; set; }
    }
}