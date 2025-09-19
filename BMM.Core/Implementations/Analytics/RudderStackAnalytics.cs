using System.Globalization;
using BMM.Api.Framework;
using BMM.Api.Implementation.Constants;
using BMM.Core.Constants;
using BMM.Core.Extensions;
using BMM.Core.Helpers;
using BMM.Core.Implementations.FirebaseRemoteConfig;
using BMM.Core.Implementations.Security;
using BMM.Core.Utils;
using RudderStack;

namespace BMM.Core.Implementations.Analytics
{
    public class RudderStackAnalytics : IAnalytics
    {
        private readonly ILogger _logger;
        private readonly IUserStorage _userStorage;
        private readonly IFirebaseRemoteConfig _remoteConfig;
        private readonly IConnection _connection;
        private static bool _isInitialized = false;

        public RudderStackAnalytics(
            ILogger logger,
            IUserStorage userStorage,
            IFirebaseRemoteConfig remoteConfig,
            IConnection connection)
        {
            _logger = logger;
            _userStorage = userStorage;
            _remoteConfig = remoteConfig;
            _connection = connection;

            InitializeIfNeeded();
        }

        private void InitializeIfNeeded()
        {
            if (_isInitialized ||
                GlobalConstants.RudderStackWriteKey.Contains(GlobalConstants.Placeholder) ||
                GlobalConstants.RudderStackDataPlaneUrl.Contains(GlobalConstants.Placeholder))
                return;

            try
            {
                var config = new RudderConfig(dataPlaneUrl: GlobalConstants.RudderStackDataPlaneUrl, async: false);
                RudderAnalytics.Initialize(GlobalConstants.RudderStackWriteKey, config);
                _isInitialized = true;
                _logger.Info("RudderStack", "RudderStack Analytics initialized successfully");
            }
            catch (Exception ex)
            {
                _logger.Error("RudderStack", $"Failed to initialize RudderStack: {ex.Message}");
            }
        }

        public void LogEvent(string eventName)
        {
            LogEvent(eventName, new Dictionary<string, object>());
        }

        public void LogEvent(string eventName, IDictionary<string, object> parameters)
        {
            if (!_isInitialized)
            {
                _logger.Warn("RudderStack", "RudderStack not initialized, skipping event");
                return;
            }

            try
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

                var dString = parameters.ToDictionary(
                    k => k.Key,
                    k =>
                    {
                        return k.Value switch
                        {
                            null => "null",
                            IFormattable formattable => formattable.ToString(null, CultureInfo.InvariantCulture),
                            _ => k.Value.ToString()
                        };
                    });

                _logger.Info("Analytics", eventName + " {" + string.Join(",", dString.Select(kv => $"{kv.Key}={kv.Value}")) + "}");

                // Get the user ID for RudderStack tracking
                var userId = user?.AnalyticsId?.ToString() ?? "anonymous";

                RudderAnalytics.Client.Track(userId, eventName, parameters);
            }
            catch (Exception ex)
            {
                _logger.Error("RudderStack", $"Failed to log event '{eventName}': {ex.Message}");
            }
        }
    }
}