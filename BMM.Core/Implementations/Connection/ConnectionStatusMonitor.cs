using System;
using System.Collections.Generic;
using BMM.Api.Framework;
using BMM.Core.Implementations.Analytics;
using BMM.Core.Implementations.Downloading;
using BMM.Core.Messages;
using MvvmCross.Plugin.Messenger;

namespace BMM.Core.Implementations.Connection
{
    public class ConnectionStatusMonitor
    {
        private readonly IAnalytics _analytics;
        private readonly IMvxMessenger _messenger;
        private readonly IConnection _connection;
        private readonly IGlobalMediaDownloader _globalMediaDownloader;
        private DateTime _lastConnectionChange;

        public ConnectionStatusMonitor(
            IAnalytics analytics,
            IMvxMessenger messenger,
            IConnection connection,
            IGlobalMediaDownloader globalMediaDownloader)
        {
            _analytics = analytics;
            _messenger = messenger;
            _connection = connection;
            _globalMediaDownloader = globalMediaDownloader;
            connection.StatusChanged += ConnectionStatusChanged;
        }

        private void ConnectionStatusChanged(object sender, ConnectionStatus connectionStatus)
        {
            PublishConnectionStatusChangedMessage(connectionStatus);
            LogConnectionStatusChanged(connectionStatus);
            LogIfConnectionStatusChangedBackAndForth(connectionStatus);
            SynchronizeOfflineTracks(connectionStatus);
        }

        private void SynchronizeOfflineTracks(ConnectionStatus connectionStatus)
        {
            if (connectionStatus == ConnectionStatus.Online
                && _connection.IsUsingNetworkWithoutExtraCosts())
            {
                _globalMediaDownloader.SynchronizeOfflineTracks();
            }
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
