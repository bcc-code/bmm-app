namespace BMM.Api.Implementation.Models
{
    public class TrackRelationArranger : TrackRelation
    {
        public TrackRelationArranger()
        {
            Type = TrackRelationType.Arranger;
        }

        public int Id { get; set; }

        public string Name { get; set; }
    }
}