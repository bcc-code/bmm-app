using System.Threading.Tasks;

namespace BMM.Core.Implementations.Notifications
{
    public interface INotificationPermissionService
    {
        Task<bool> CheckIsNotificationPermissionGranted();
        Task RequestNotificationPermission(bool fallbackToSettings);
    }
}