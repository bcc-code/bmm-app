using System;
using System.Collections;
using System.Collections.Generic;
using Xamarin.Essentials;

namespace BMM.Api.Framework
{
    public interface IConnection
    {
        event EventHandler<ConnectionStatus> StatusChanged;

        ConnectionStatus GetStatus();

        bool IsUsingNetworkWithoutExtraCosts();

        IEnumerable<ConnectionProfile> GetActiveConnectionProfiles();
    }
}