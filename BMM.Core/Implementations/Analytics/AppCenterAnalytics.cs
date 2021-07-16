using System.Collections.Generic;
using System.Linq;
using BMM.Api.Framework;
using BMM.Api.Implementation.Models;
using BMM.Core.Implementations.FirebaseRemoteConfig;
using BMM.Core.Implementations.Security;

namespace BMM.Core.Implementations.Analytics
{
    public class AppCenterAnalytics : IAnalytics
    {
        private readonly ILogger _logger;
        private readonly IUserStorage _userStorage;
        private readonly IFirebaseRemoteConfig _remoteConfig;

        public AppCenterAnalytics(ILogger logger, IUserStorage userStorage, IFirebaseRemoteConfig remoteConfig)
        {
            _logger = logger;
            _userStorage = userStorage;
            _remoteConfig = remoteConfig;
        }

        public void LogEvent(string eventName)
        {
            Microsoft.AppCenter.Analytics.Analytics.TrackEvent(eventName);
        }

        public void LogEvent(string eventName, IDictionary<string, object> parameters)
        {
            var user = _userStorage.GetUser();
            if (_remoteConfig.UseAnalyticsIdentifier)
            {
                if (!parameters.ContainsKey(nameof(User.AnalyticsIdentifier)) && user != null)
                    parameters.Add(nameof(User.AnalyticsIdentifier), user.AnalyticsIdentifier);
            }
            else
            {
                if (!parameters.ContainsKey(nameof(user.PersonId)) && user != null)
                    parameters.Add(nameof(user.PersonId), user.PersonId);
            }
            Dictionary<string, string> dString = parameters.ToDictionary(k => k.Key, k => k.Value == null ? "null" : k.Value.ToString());
            _logger.Info("Analytics", eventName + " {" + string.Join(",", dString.Select(kv => $"{kv.Key}={kv.Value}")) + "}");
            Microsoft.AppCenter.Analytics.Analytics.TrackEvent(eventName, dString);
        }
    }
}