using BMM.Core.ViewModels.Parameters.Interface;

namespace BMM.Core.ViewModels.Parameters
{
    public class TrackCollectionParameter : ITrackCollectionParameter
    {
        public TrackCollectionParameter(
            int trackCollectionId,
            string name = "",
            bool useLikeIcon = false)
        {
            TrackCollectionId = trackCollectionId;
            Name = name;
            UseLikeIcon = useLikeIcon;
        }

        public int TrackCollectionId { get; }
        public string Name { get; }
        public bool UseLikeIcon { get; }
    }
}