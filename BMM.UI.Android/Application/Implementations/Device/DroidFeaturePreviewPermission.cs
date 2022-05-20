using BMM.Core.Implementations.Device;

namespace BMM.UI.Droid.Application.Implementations.Device
{
    public class DroidFeaturePreviewPermission : IFeatureSupportInfoService
    {
        public bool SupportsDarkMode => true;
        public bool SupportsSiri => false;
    }
}