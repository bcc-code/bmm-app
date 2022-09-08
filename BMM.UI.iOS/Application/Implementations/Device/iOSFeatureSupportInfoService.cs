using System.Linq;
using BMM.Core.Implementations.Device;
using BMM.Core.Implementations.Languages;
using BMM.UI.iOS.Constants;
using UIKit;

namespace BMM.UI.iOS.Implementations.Device
{
    public class iOSFeatureSupportInfoService : IFeatureSupportInfoService
    {
        private readonly IAppLanguageProvider _appLanguageProvider;

        public iOSFeatureSupportInfoService(IAppLanguageProvider appLanguageProvider)
        {
            _appLanguageProvider = appLanguageProvider;
        }
        
        public bool SupportsDarkMode => UIDevice.CurrentDevice.CheckSystemVersion(13, 0);
        public bool SupportsSiri
        {
            get
            {
#if UI_TESTS
                return false;
#endif
                
                return UIDevice.CurrentDevice.CheckSystemVersion(13, 0)
                       && SiriConstants.AvailableLanguages.Contains(_appLanguageProvider.GetAppLanguage());
            }
        }

        public bool SupportsAVPlayerItemCache => UIDevice.CurrentDevice.CheckSystemVersion(13, 0);
    }
}