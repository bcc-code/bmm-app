using BMM.Core.ViewModels.Parameters.Interface;

namespace BMM.Core.ViewModels.Parameters
{
    public class SharedTrackCollectionParameter : ISharedTrackCollectionParameter
    {
        public SharedTrackCollectionParameter(string sharingSecret)
        {
            SharingSecret = sharingSecret;
        }

        public string SharingSecret { get; }
    }
}