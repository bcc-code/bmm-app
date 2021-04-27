namespace BMM.Core.Helpers
{
    public class SupportVersionChecker
    {
        private readonly IDeviceSupportVersionChecker _deviceSupportVersionChecker;
        private readonly AppSupportVersionChecker _appSupportVersionChecker;

        public SupportVersionChecker(IDeviceSupportVersionChecker deviceSupportVersionChecker,
            AppSupportVersionChecker appSupportVersionChecker)
        {
            _deviceSupportVersionChecker = deviceSupportVersionChecker;
            _appSupportVersionChecker = appSupportVersionChecker;
        }

        public bool IsCurrentDeviceVersionSupported()
        {
            return _deviceSupportVersionChecker.IsCurrentDeviceVersionSupported();
        }

        public bool IsCurrentDeviceVersionPlannedToBeUnsupported()
        {
            return _deviceSupportVersionChecker.IsCurrentDeviceVersionPlannedToBeUnsupported();
        }

        public bool IsCurrentAppVersionSupported()
        {
            return _appSupportVersionChecker.IsCurrentAppVersionSupported();
        }

        public bool IsCurrentAppVersionPlannedToBeUnsupported()
        {
            return _appSupportVersionChecker.IsCurrentAppVersionPlannedToBeUnsupported();
        }

        public bool DeviceIsSupportedButWillBeUnsupported()
        {
            return IsCurrentDeviceVersionPlannedToBeUnsupported() && IsCurrentDeviceVersionSupported();
        }

        public bool AppVersionIsSupportedButWillBeUnsupported()
        {
            return IsCurrentAppVersionPlannedToBeUnsupported() && IsCurrentAppVersionSupported();
        }
    }
}
