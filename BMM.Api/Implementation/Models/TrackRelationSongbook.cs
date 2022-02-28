using BMM.Api.Utils;
using Newtonsoft.Json;

namespace BMM.Api.Implementation.Models
{
    public class TrackRelationSongbook : TrackRelation
    {
        public TrackRelationSongbook()
        {
            Type = TrackRelationType.Songbook;
        }

        public string Comment { get; set; }

        public int Id { get; set; }

        public string Name { get; set; }

        public int Timestamp { get; set; }

        [JsonIgnore]
        public string ShortName => $"{SongbookUtils.GetShortName(Name)} {Id}";
    }
}