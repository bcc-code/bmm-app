using BMM.Api.Framework;
using BMM.Core.Constants;
using BMM.Core.Implementations.Logger;
using BMM.Core.Implementations.Security;
using BMM.UI.iOS.Extensions;
using iOS.NewRelic;

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
            NewRelic.RecordCustomEvent(AnalyticsConstants.NewRelicEventType, $"{message}", properties.ToNSDictionary());
        }
    }
}