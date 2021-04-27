namespace BMM.Core.Implementations.Notifications.Data
{
    public class WordOfFaithNotification : LocalNotification
    {
        public const string UrlKey = "word-of-faith_url";

        public const string Type = "word-of-faith_notification";

        public string Url { get; set; }

        public string CancelText { get; set; }

        public string YesText { get; set; }

        public override string ChannelId => "org.brunstad.bmm.WORDS_OF_Faith";

        public override string ChannelName => "Words of faith";

        public override string ChannelDescription => "Reminder to answer the questions";
    }
}