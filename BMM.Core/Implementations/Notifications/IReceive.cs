namespace BMM.Core.Implementations.Notifications
{
    public interface IReceive<in T> where T : RemoteNotification
    {
        void UserClickedRemoteNotification(T notification);

        void OnNotificationReceived(T notification);
    }

    public interface IReceiveLocal<in T> where T : LocalNotification
    {
        void UserClickedLocalNotification(T notification);
    }
}