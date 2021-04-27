using System.Threading.Tasks;
using Acr.UserDialogs;
using BMM.Core.Implementations.FirebaseRemoteConfig;
using BMM.Core.Implementations.Security;
using BMM.Core.Implementations.Startup;
using MvvmCross.Localization;
using Xamarin.Essentials;

namespace BMM.Core.Helpers
{
    public class AfterStartupSupportEndsPopupDisplayer : IDelayedStartupTask
    {
        private readonly IUserDialogs _userDialogs;
        private readonly IFirebaseRemoteConfig _remoteConfig;
        private readonly IUserAuthChecker _userAuthChecker;
        private readonly SupportVersionChecker _supportVersionChecker;

        public AfterStartupSupportEndsPopupDisplayer(IUserDialogs userDialogs, IFirebaseRemoteConfig remoteConfig, IUserAuthChecker userAuthChecker,
            SupportVersionChecker supportVersionChecker)
        {
            _userDialogs = userDialogs;
            _remoteConfig = remoteConfig;
            _userAuthChecker = userAuthChecker;
            _supportVersionChecker = supportVersionChecker;
        }

        public async Task RunAfterStartup()
        {
            await ShowAlertIfCurrentDeviceVersionIsPlannedToBeUnsupported();
            await ShowAlertIfCurrentAppVersionIsPlannedToBeUnsupported();
        }

        private async Task ShowAlertIfCurrentDeviceVersionIsPlannedToBeUnsupported()
        {
            if (_supportVersionChecker.DeviceIsSupportedButWillBeUnsupported())
            {
                var deviceSupportEndsMessage = new MvxLanguageBinder(GlobalConstants.GeneralNamespace, typeof(ViewModels.SupportEndedViewModel).Name)
                    .GetText("DeviceSupportEnding", DeviceInfo.Platform.ToString(), ChoseCurrentPlatformVersionToBeUnsupported());

                var userIsAuthenticated = await _userAuthChecker.IsUserAuthenticated();
                if(userIsAuthenticated) await _userDialogs.AlertAsync(deviceSupportEndsMessage);
            }
        }

        private string ChoseCurrentPlatformVersionToBeUnsupported()
        {
            if (DeviceInfo.Platform == DevicePlatform.Android)
                return _remoteConfig.AndroidVersionPlannedToBeUnsupported.Version;

            return _remoteConfig.IosVersionPlannedToBeUnsupported.Version;
        }

        private async Task ShowAlertIfCurrentAppVersionIsPlannedToBeUnsupported()
        {
            if(_supportVersionChecker.AppVersionIsSupportedButWillBeUnsupported())
            {
                var appSupportEndsMessage = new MvxLanguageBinder(GlobalConstants.GeneralNamespace, typeof(ViewModels.SupportEndedViewModel).Name)
                    .GetText("AppSupportEnding");

                var userIsAuthenticated = await _userAuthChecker.IsUserAuthenticated();
                if(userIsAuthenticated) await _userDialogs.AlertAsync(appSupportEndsMessage);
            }
        }
    }
}
