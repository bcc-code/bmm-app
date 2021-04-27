using System;
using System.Collections.Generic;
using BMM.Api.Framework;
using BMM.Core.Implementations.Analytics;
using BMM.Core.Messages;
using MvvmCross.Plugin.Messenger;

namespace BMM.Core.Implementations.Connection
{
    public class ConnectionStatusLogger
    {
        private readonly IAnalytics _analytics;
        private readonly IMvxMessenger _messenger;
        private DateTime _lastConnectionChange;

        public ConnectionStatusLogger(IAnalytics analytics, IMvxMessenger messenger, IConnection connection)
        {
            _analytics = analytics;
            _messenger = messenger;
            connection.StatusChanged += ConnectionStatusChanged;
        }

        private void ConnectionStatusChanged(object sender, ConnectionStatus connectionStatus)
        {
            PublishConnectionStatusChangedMessage(connectionStatus);
            LogConnectionStatusChanged(connectionStatus);
            LogIfConnectionStatusChangedBackAndForth(connectionStatus);
        }

        private void PublishConnectionStatusChangedMessage(ConnectionStatus connectionStatus)
        {
            var message = new ConnectionStatusChangedMessage(this, connectionStatus);
            _messenger.Publish(message);
        }

        private void LogConnectionStatusChanged(ConnectionStatus connectionStatus)
        {
            _analytics.LogEvent("The connection status changed",
                new Dictionary<string, object> {{"newStatus", connectionStatus}});
        }

        private void LogIfConnectionStatusChangedBackAndForth(ConnectionStatus connectionStatus)
        {
            var timeElapsed = DateTime.UtcNow - _lastConnectionChange;

            if (timeElapsed.TotalSeconds < 30)
            {
                _analytics.LogEvent("The connection changed back and forth",
                    new Dictionary<string, object>
                    {
                        {"newStatus", connectionStatus},
                        {"timeElapsedInSeconds", timeElapsed.TotalSeconds}
                    });
            }

            _lastConnectionChange = DateTime.UtcNow;
        }
    }
}
