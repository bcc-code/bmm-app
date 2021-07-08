using System.Collections.Generic;
using Newtonsoft.Json;

namespace BMM.Api.Implementation.Models
{
    [JsonObject]
    public class TrackCollection : BaseTrackCollection
    {
        private int _trackCount;

        public int TrackCount { get => Tracks?.Count > 0 ? Tracks.Count : _trackCount; set => _trackCount = value; }

        public int FollowerCount { get; set; }
        public string ShareLink { get; set; }
        public string AuthorName { get; set; }
        public bool CanEdit { get; set; }
        public List<Track> Tracks { get; set; }
    }
}