namespace BMM.Core.Implementations.Notifications
{
    public interface INotification
    {
    }

    public abstract class RemoteNotification : INotification
    {
    }

    public abstract class LocalNotification : INotification
    {
        // ToDo: Once we move to C# 8 we should move to INotification
        public const string TypeKey = "type";

        public string Message { get; set; }
        public string Title { get; set; }

        /// <summary>
        /// The channel information is only used by android.
        /// ToDo: Make channel information translatable
        /// </summary>
        public abstract string ChannelId { get; }

        public abstract string ChannelName { get; }

        public abstract string ChannelDescription { get; }
    }
}