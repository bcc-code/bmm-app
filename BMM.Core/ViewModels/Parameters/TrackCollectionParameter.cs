using BMM.Core.ViewModels.Parameters.Interface;

namespace BMM.Core.ViewModels.Parameters
{
    public class TrackCollectionParameter : ITrackCollectionParameter
    {
        public TrackCollectionParameter(int trackCollectionId)
        {
            TrackCollectionId = trackCollectionId;
        }

        public int TrackCollectionId { get; }
    }
}