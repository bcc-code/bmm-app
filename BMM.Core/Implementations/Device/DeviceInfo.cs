using Xamarin.Essentials;

namespace BMM.Core.Implementations.Device
{
    public class DeviceInfo : IDeviceInfo
    {
        public string Manufacturer => Xamarin.Essentials.DeviceInfo.Manufacturer;

        public string Model => Xamarin.Essentials.DeviceInfo.Model;

        public BmmDevicePlatform Platform
        {
            get
            {
                if (Xamarin.Essentials.DeviceInfo.Platform == DevicePlatform.Android)
                    return BmmDevicePlatform.Android;
                if (Xamarin.Essentials.DeviceInfo.Platform == DevicePlatform.iOS)
                    return BmmDevicePlatform.iOS;

                return BmmDevicePlatform.Unsupported;
            }
        }

        public bool IsAndroid => Platform == BmmDevicePlatform.Android;

        public bool IsIos => Platform == BmmDevicePlatform.iOS;

        public string VersionString => Xamarin.Essentials.DeviceInfo.VersionString;
    }
}