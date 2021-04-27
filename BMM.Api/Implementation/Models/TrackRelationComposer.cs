namespace BMM.Api.Implementation.Models
{
    public class TrackRelationComposer : TrackRelation
    {
        public TrackRelationComposer()
        {
            Type = TrackRelationType.Composer;
        }

        public int Id { get; set; }

        public string Name { get; set; }
    }
}