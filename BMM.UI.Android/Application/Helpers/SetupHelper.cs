using BMM.Core.Helpers;
using MvvmCross.Platforms.Android.Core;

namespace BMM.UI.Droid.Application.Helpers
{
    public class SetupHelper
    {
        /// <summary>
        /// This has to be called at every entry point of the application to make sure AppCenter and Mvx are initialized
        /// </summary>
        public static void EnsureInitialized()
        {
            AppCenterHelper.DroidRegister();
            var setup = MvxAndroidSetupSingleton.EnsureSingletonAvailable(BmmApplication.Instance);
            setup.EnsureInitialized();
        }
    }
}