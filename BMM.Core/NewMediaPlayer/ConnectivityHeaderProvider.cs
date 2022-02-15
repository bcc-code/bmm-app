using System.Collections.Generic;
using System.Threading.Tasks;
using BMM.Api.Abstraction;
using BMM.Api.Framework;

namespace BMM.Core.NewMediaPlayer
{
    public class ConnectivityHeaderProvider : IHeaderProvider
    {
        private readonly IConnection _connection;

        public ConnectivityHeaderProvider(IConnection connection)
        {
            _connection = connection;
        }

        public async Task<KeyValuePair<string, string>?> GetHeader()
        {
            var value = _connection.IsUsingNetworkWithoutExtraCosts() ? "Wifi" : "Mobile";
            return new KeyValuePair<string, string>("AppConnectivity", value);
        }
    }
}