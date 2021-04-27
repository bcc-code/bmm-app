namespace BMM.Api.Implementation.Models
{
    public class TrackRelationLyricist : TrackRelation
    {
        public TrackRelationLyricist()
        {
            Type = TrackRelationType.Lyricist;
        }

        public int Id { get; set; }

        public string Name { get; set; }
    }
}