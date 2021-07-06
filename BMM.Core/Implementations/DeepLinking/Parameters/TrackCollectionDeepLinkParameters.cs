using BMM.Core.Implementations.DeepLinking.Base.Interfaces;

namespace BMM.Core.Implementations.DeepLinking.Parameters
{
    public class TrackCollectionDeepLinkParameters : IDeepLinkParameters
    {
        public string SharingSecret { get; set; }
    }
}