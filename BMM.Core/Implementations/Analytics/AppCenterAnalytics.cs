using System.Collections.Generic;
using System.Linq;
using BMM.Api.Framework;
using BMM.Api.Implementation.Models;
using BMM.Core.Implementations.Security;

namespace BMM.Core.Implementations.Analytics
{
    public class AppCenterAnalytics : IAnalytics
    {
        private readonly ILogger _logger;
        private readonly IUserStorage _userStorage;

        public AppCenterAnalytics(ILogger logger, IUserStorage userStorage)
        {
            _logger = logger;
            _userStorage = userStorage;
        }

        public void LogEvent(string eventName)
        {
            Microsoft.AppCenter.Analytics.Analytics.TrackEvent(eventName);
        }

        public void LogEvent(string eventName, IDictionary<string, object> parameters)
        {
            var user = _userStorage.GetUser();
            if (!parameters.ContainsKey(nameof(User.AnalyticsIdentifier)) && user != null)
                parameters.Add(nameof(User.AnalyticsIdentifier), _userStorage.GetUser().AnalyticsIdentifier);

            Dictionary<string, string> dString = parameters.ToDictionary(k => k.Key, k => k.Value == null ? "null" : k.Value.ToString());
            _logger.Info("Analytics", eventName + " {" + string.Join(",", dString.Select(kv => $"{kv.Key}={kv.Value}")) + "}");
            Microsoft.AppCenter.Analytics.Analytics.TrackEvent(eventName, dString);
        }
    }
}