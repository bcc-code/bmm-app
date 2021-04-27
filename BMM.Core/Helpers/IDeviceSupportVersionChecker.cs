namespace BMM.Core.Helpers
{
    public interface IDeviceSupportVersionChecker
    {
        bool IsCurrentDeviceVersionSupported();

        bool IsCurrentDeviceVersionPlannedToBeUnsupported();
    }
}
