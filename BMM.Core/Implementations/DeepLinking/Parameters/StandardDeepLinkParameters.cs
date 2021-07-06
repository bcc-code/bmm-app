using BMM.Core.Implementations.DeepLinking.Base.Interfaces;

namespace BMM.Core.Implementations.DeepLinking.Parameters
{
    public class StandardDeepLinkParameters : IDeepLinkParameters
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}