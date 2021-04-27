namespace BMM.Core.Implementations.Notifications
{
    public interface INotificationDisplayer
    {
        void DisplayNotificationOrPopup(LocalNotification notification);
    }
}