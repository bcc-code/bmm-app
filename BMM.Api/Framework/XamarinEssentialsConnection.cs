using System;
using System.Linq;
using Xamarin.Essentials;

namespace BMM.Api.Framework
{
    public class XamarinEssentialsConnection : IConnection
    {
        public XamarinEssentialsConnection()
        {
            Connectivity.ConnectivityChanged += (sender, args) => { StatusChanged?.Invoke(this, GetStatus()); };
        }

        public event EventHandler<ConnectionStatus> StatusChanged;

        public ConnectionStatus GetStatus()
        {
            return Connectivity.NetworkAccess == NetworkAccess.Internet ? ConnectionStatus.Online : ConnectionStatus.Offline;
        }

        /// <summary>
        /// iOS can just have one value. Either <see cref="ConnectionProfile.Cellular"/> or <see cref="ConnectionProfile.WiFi"/> or <see cref="ConnectionProfile.Unknown"/>.
        /// Basically it's just checking if it has WiFi, mobile or no connection.
        /// Android on the other hand can have multiple values. This means if the device has WiFi and mobile connection there will be two connection profiles.
        /// </summary>
        public bool IsUsingNetworkWithoutExtraCosts()
        {
            return Connectivity.ConnectionProfiles.Contains(ConnectionProfile.WiFi)
                   || Connectivity.ConnectionProfiles.Contains(ConnectionProfile.Ethernet)
                   || Connectivity.ConnectionProfiles.Contains(ConnectionProfile.Bluetooth);
        }
    }
}