using BMM.Api.Implementation.Models.Enums;

namespace BMM.Core.Implementations.Device
{
    public abstract class BaseDeviceInfo : IDeviceInfo
    {
        public string Manufacturer => Microsoft.Maui.Devices.DeviceInfo.Manufacturer;

        public string Model => Microsoft.Maui.Devices.DeviceInfo.Model;

        public BmmDevicePlatform Platform
        {
            get
            {
                if (Microsoft.Maui.Devices.DeviceInfo.Platform == Microsoft.Maui.Devices.DevicePlatform.Android)
                    return BmmDevicePlatform.Android;
                if (Microsoft.Maui.Devices.DeviceInfo.Platform == Microsoft.Maui.Devices.DevicePlatform.iOS)
                    return BmmDevicePlatform.iOS;

                return BmmDevicePlatform.Unsupported;
            }
        }

        public bool IsAndroid => Platform == BmmDevicePlatform.Android;

        public bool IsIos => Platform == BmmDevicePlatform.iOS;

        public string VersionString => Microsoft.Maui.Devices.DeviceInfo.VersionString;
        public abstract Task<AppTheme> GetCurrentTheme();
    }
}