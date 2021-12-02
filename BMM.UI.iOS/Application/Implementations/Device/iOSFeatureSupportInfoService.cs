using BMM.Core.Implementations.Device;
using UIKit;

namespace BMM.UI.iOS.Implementations.Device
{
    public class iOSFeatureSupportInfoService : IFeatureSupportInfoService
    {
        public bool SupportsDarkMode => UIDevice.CurrentDevice.CheckSystemVersion(13, 0);
    }
}