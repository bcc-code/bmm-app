using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using BMM.Api.Framework.HTTP;
using UIKit;

namespace BMM.UI.iOS
{
    public class NetworkAccessAwareHttpClient: ISimpleHttpClient
    {
        private readonly HttpClient _httpClient;
        private static int _requestCounter = 0;

        public NetworkAccessAwareHttpClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
        {
            NetworkRequestStarted();

            HttpResponseMessage response;
            try
            {
                response = await _httpClient.SendAsync(request);
            }
            finally
            {
                NetworkRequestCompleted();
            }

            return response;
        }

        public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            NetworkRequestStarted();

            HttpResponseMessage response;
            try
            {
                response = await _httpClient.SendAsync(request, cancellationToken);
            }
            finally
            {
                NetworkRequestCompleted();
            }

            return response;
        }

        private void NetworkRequestStarted()
        {
            _requestCounter++;
            UIApplication.SharedApplication.NetworkActivityIndicatorVisible = true;
        }

        private void NetworkRequestCompleted()
        {
            _requestCounter--;
            if (_requestCounter == 0)
            {
                UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false;
            }
        }
    }
}