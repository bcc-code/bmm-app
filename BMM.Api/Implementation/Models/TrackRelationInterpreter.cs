namespace BMM.Api.Implementation.Models
{
    public class TrackRelationInterpreter : TrackRelation
    {
        public TrackRelationInterpreter()
        {
            Type = TrackRelationType.Interpret;
        }

        public int Id { get; set; }

        public string Name { get; set; }
    }
}