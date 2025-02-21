using BMM.Core.Implementations.DeepLinking.Base.Interfaces;

namespace BMM.Core.Implementations.DeepLinking.Parameters;

public class StringIdParameters : IDeepLinkParameters
{
    public string Id { get; set; }
}