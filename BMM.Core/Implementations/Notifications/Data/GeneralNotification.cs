namespace BMM.Core.Implementations.Notifications.Data
{
    public class GeneralNotification : RemoteNotification
    {
        public const string Type = "general-information_notification";

        public const string ActionUrlKey = "action_url";

        public string ActionUrl { get; set; }
    }
}