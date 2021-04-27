namespace BMM.Api.Implementation.Models
{
    public class TrackRelationBible : TrackRelation
    {
        public TrackRelationBible()
        {
            Type = TrackRelationType.Bible;
        }

        public string Book { get; set; }

        public int Chapter { get; set; }

        public string Comment { get; set; }

        public int Timestamp { get; set; }

        public int Verse { get; set; }
    }
}