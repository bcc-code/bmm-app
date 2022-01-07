using System;
using BMM.Core.Helpers;
using BMM.Core.Implementations.Device;
using BMM.Core.Implementations.FirebaseRemoteConfig;
using BMM.Core.Implementations.UI;
using BMM.Core.Models;
using BMM.Core.Translation;
using BMM.Core.ViewModels.Base;
using MvvmCross.Commands;

namespace BMM.Core.ViewModels
{
    public struct SupportEndedParameters
    {
        public SupportEndedMessage SupportEndedMessage { get; set; }
    }

    public class SupportEndedViewModel : BaseViewModel<SupportEndedParameters>
    {
        private readonly IDeviceInfo _deviceInfo;
        private readonly IFirebaseRemoteConfig _remoteConfig;
        private readonly IUriOpener _uriOpener;

        public IMvxCommand ShowAppUpdatePageCommand { get; }

        public string SupportEndedInfo
        {
            get => ChooseSupportEndedMessage();
        }

        public bool ShouldShowAppUpdateButton => CurrentPageIsAppSupportEndedPage();

        public SupportEndedViewModel(IDeviceInfo deviceInfo, IFirebaseRemoteConfig remoteConfig, IUriOpener uriOpener)
        {
            _deviceInfo = deviceInfo;
            _remoteConfig = remoteConfig;
            _uriOpener = uriOpener;

            ShowAppUpdatePageCommand = new MvxCommand(ShowAppUpdatePage);
        }

        private string ChooseSupportEndedMessage()
        {
            switch (NavigationParameter.SupportEndedMessage)
            {
                case SupportEndedMessage.DeviceSupportEnded:
                    return TextSource.GetText(Translations.SupportEndedViewModel_DeviceSupportEnded, _deviceInfo.Platform, ChoseCurrentPlatformMinimumRequiredDeviceVersion());
                case SupportEndedMessage.ApplicationVersionSupportEnded:
                    return TextSource[Translations.SupportEndedViewModel_AppSupportEnded];
                default:
                    throw new Exception("Couldn't get text for Support Ended Info.");
            }
        }

        private string ChoseCurrentPlatformMinimumRequiredDeviceVersion()
        {
            if (_deviceInfo.IsAndroid) return _remoteConfig.MinimumRequiredAndroidVersion.Version;
            else if (_deviceInfo.IsIos) return _remoteConfig.MinimumRequiredIosVersion.Version;
            throw new Exception("Device platform is unsupported platform.");
        }

        private void ShowAppUpdatePage()
        {
            if (_deviceInfo.IsAndroid) _uriOpener.OpenUri(new Uri(GlobalConstants.AndroidUpdateLink));
            else if (_deviceInfo.IsIos) _uriOpener.OpenUri(new Uri(GlobalConstants.IosUpdateLink));
        }

        private bool CurrentPageIsAppSupportEndedPage()
        {
            return NavigationParameter.SupportEndedMessage == SupportEndedMessage.ApplicationVersionSupportEnded;
        }
    }
}
