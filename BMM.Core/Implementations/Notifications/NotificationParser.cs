using BMM.Api.Framework;
using BMM.Core.Implementations.Notifications.Data;

namespace BMM.Core.Implementations.Notifications
{
    public class NotificationParser
    {
        private readonly ILogger _logger;

        public NotificationParser(ILogger logger)
        {
            _logger = logger;
        }

        public INotification ParseNotification(IPlatformNotification message)
        {
            if (message.ContainsKey(LocalNotification.TypeKey))
            {
                var type = message.GetString(LocalNotification.TypeKey);

                switch (type)
                {
                    case PodcastNotification.Type:
                    {
                        var ids = message.GetIntList(PodcastNotification.TrackIdsKey);
                        return new PodcastNotification(message.GetInt(PodcastNotification.PodcastIdKey), ids);
                    }
                    case GeneralNotification.Type:
                    {
                        var url = message.GetString(GeneralNotification.ActionUrlKey);
                        return new GeneralNotification {ActionUrl = url};
                    }
                    default:
                        _logger.Error(nameof(NotificationParser), $"Not able to parse notification unknown type: {type}");
                        return null;
                }
            }

            _logger.Error(nameof(NotificationParser), "Not able to parse notification contains no type");

            return null;
        }
    }
}