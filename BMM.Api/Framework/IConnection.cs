using System;

namespace BMM.Api.Framework
{
    public interface IConnection
    {
        event EventHandler<ConnectionStatus> StatusChanged;

        ConnectionStatus GetStatus();

        bool IsUsingNetworkWithoutExtraCosts();
    }
}