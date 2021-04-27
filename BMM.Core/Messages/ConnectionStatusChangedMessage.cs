using BMM.Api.Framework;
using MvvmCross.Plugin.Messenger;

namespace BMM.Core.Messages
{
    public class ConnectionStatusChangedMessage : MvxMessage
    {
        public ConnectionStatusChangedMessage(object sender, ConnectionStatus connectionStatus) : base(sender)
        {
            ConnectionStatus = connectionStatus;
        }

        public ConnectionStatus ConnectionStatus { get; }
    }
}
