using System.Collections.Generic;
using System.Linq;
using BMM.Api.Framework;
using BMM.Api.Implementation.Constants;
using BMM.Core.Constants;
using BMM.Core.Extensions;
using BMM.Core.Implementations.FirebaseRemoteConfig;
using BMM.Core.Implementations.Security;
using BMM.Core.Utils;

namespace BMM.Core.Implementations.Analytics
{
    public class AppCenterAnalytics : IAnalytics
    {
        private readonly ILogger _logger;
        private readonly IUserStorage _userStorage;
        private readonly IFirebaseRemoteConfig _remoteConfig;
        private readonly IConnection _connection;

        public AppCenterAnalytics(
            ILogger logger,
            IUserStorage userStorage,
            IFirebaseRemoteConfig remoteConfig,
            IConnection connection)
        {
            _logger = logger;
            _userStorage = userStorage;
            _remoteConfig = remoteConfig;
            _connection = connection;
        }

        public void LogEvent(string eventName)
        {
            LogEvent(eventName, new Dictionary<string, object>());
        }

        public void LogEvent(string eventName, IDictionary<string, object> parameters)
        {
            var user = _userStorage.GetUser();
            if (user != null)
            {
                parameters.AddIfNew(nameof(user.AnalyticsId), user.AnalyticsId);

                if (user.Age != null)
                    parameters.Add(nameof(user.Age), user.Age);
            }

            if (!string.IsNullOrEmpty(_remoteConfig.ExperimentId))
                parameters.AddIfNew(HeaderNames.ExperimentId, _remoteConfig.ExperimentId);
            
            parameters.Add(AnalyticsConstants.ConnectionParameterName, AnalyticsUtils.GetConnectionType(_connection));

            Dictionary<string, string> dString = parameters.ToDictionary(k => k.Key, k => k.Value == null ? "null" : k.Value.ToString());
            _logger.Info("Analytics", eventName + " {" + string.Join(",", dString.Select(kv => $"{kv.Key}={kv.Value}")) + "}");
            Microsoft.AppCenter.Analytics.Analytics.TrackEvent(eventName, dString);
        }
    }
}