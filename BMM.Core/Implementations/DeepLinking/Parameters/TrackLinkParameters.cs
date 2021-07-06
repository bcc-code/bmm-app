using BMM.Core.Implementations.DeepLinking.Base.Interfaces;

namespace BMM.Core.Implementations.DeepLinking.Parameters
{
    public class TrackLinkParameters : IDeepLinkParameters
    {
        public int Id { get; set; }
        public long StartTimeInMs { get; set; }
    }
}