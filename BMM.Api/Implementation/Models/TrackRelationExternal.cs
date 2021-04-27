namespace BMM.Api.Implementation.Models
{
    public class TrackRelationExternal : TrackRelation
    {
        public TrackRelationExternal()
        {
            Type = TrackRelationType.External;
        }

        public string Name { get; set; }

        public string Url { get; set; }
    }
}