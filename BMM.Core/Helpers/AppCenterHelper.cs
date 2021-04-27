using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;

namespace BMM.Core.Helpers
{
    public abstract class AppCenterHelper
    {
        public static void DroidRegister()
        {
            if (!AppCenter.Configured)
                AppCenter.Start(GlobalConstants.DroidAppSecret, typeof(Analytics), typeof(Crashes));
        }

        public static void IOSRegister()
        {
            if (!AppCenter.Configured)
                AppCenter.Start(GlobalConstants.iOSAppSecret, typeof(Analytics), typeof(Crashes));
        }
    }
}