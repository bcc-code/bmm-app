using BMM.Api.Framework;
using BMM.Core.Constants;
using BMM.Core.Implementations.Logger;
using BMM.Core.Implementations.Security;
using Java.Lang;
using Exception = System.Exception;
using Object = Java.Lang.Object;

namespace BMM.UI.Droid.Application.Implementations
{
    public class AndroidLogger : BaseLogger
    {
        public AndroidLogger(
            IUserStorage userStorage,
            IConnection connection) 
            : base(connection, userStorage)
        {
        }
        
        public override void Error(string tag, string message, Exception exception, bool presentedToUser = false)
        {
            var throwableException = Throwable.FromException(exception);
            base.Error(tag, message, throwableException, presentedToUser);
        }

        public override void TrackEvent(string message, IDictionary<string, string> properties)
        {
            Console.WriteLine($"EVENT - {message}");
            Com.Newrelic.Agent.Android.NewRelic.RecordCustomEvent(AnalyticsConstants.NewRelicEventType, $"{message}", properties.ToDictionary(k => k.Key, v => (Object)v.Value));
        }
    }
}