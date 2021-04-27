using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace BMM.Api.Implementation.Models
{
    [JsonObject]
    public class TrackCollectionReference : BaseTrackCollection
    {
        public TrackCollectionReference(TrackCollection collection)
        {
            TrackReferences = collection.Tracks != null
                ? collection.Tracks.Select(track => new TrackReference {Id = track.Id, Language = track.Language}).ToList()
                : new List<TrackReference>();
            Name = collection.Name;
            Access = collection.Access;
            Id = collection.Id;
        }

        public List<TrackReference> TrackReferences { get; set; }
    }
}