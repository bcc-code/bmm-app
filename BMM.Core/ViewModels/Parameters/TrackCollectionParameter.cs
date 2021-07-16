using BMM.Core.ViewModels.Parameters.Interface;

namespace BMM.Core.ViewModels.Parameters
{
    public class TrackCollectionParameter : ITrackCollectionParameter
    {
        public TrackCollectionParameter(
            int trackCollectionId,
            string name = "")
        {
            TrackCollectionId = trackCollectionId;
            Name = name;
        }

        public int TrackCollectionId { get; }
        public string Name { get; }
    }
}