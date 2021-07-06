using BMM.Core.ViewModels.Parameters.Interface;

namespace BMM.Core.ViewModels.Parameters
{
    public class TrackCollectionParameter : ITrackCollectionParameter
    {
        public TrackCollectionParameter(
            int? trackCollectionId = null,
            string name = "",
            string sharingSecret = "")
        {
            TrackCollectionId = trackCollectionId;
            Name = name;
            SharingSecret = sharingSecret;
        }

        public int? TrackCollectionId { get; }
        public string Name { get; }
        public string SharingSecret { get; }
    }
}