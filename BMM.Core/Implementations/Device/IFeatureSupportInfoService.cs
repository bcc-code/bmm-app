namespace BMM.Core.Implementations.Device
{
    public interface IFeatureSupportInfoService
    {
        bool SupportsDarkMode { get; }
        
        bool SupportsSiri { get; }
        
        bool SupportsAVPlayerItemCache { get; }
    }
}