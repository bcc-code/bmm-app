using BMM.Api.Framework;
using BMM.Core.Implementations.Logger;
using BMM.Core.Implementations.Security;
using RudderStack;

namespace BMM.UI.iOS.Implementations
{
    public class IosLogger : BaseLogger
    {
        public IosLogger(
            IUserStorage userStorage,
            IConnection connection)
            : base(connection, userStorage)
        {
        }

        public override void TrackEvent(string message, IDictionary<string, string> properties)
        {
            Console.WriteLine($"EVENT - {message}");

            var user = UserStorage.GetUser();
            var userId = user?.AnalyticsId?.ToString() ?? "anonymous";
            var eventProperties = properties.ToDictionary(k => k.Key, v => (object)v.Value);

            RudderAnalytics.Client?.Track(userId, message, eventProperties);
        }
    }
}