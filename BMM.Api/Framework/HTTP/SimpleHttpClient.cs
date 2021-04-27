using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace BMM.Api.Framework.HTTP
{
    public class SimpleHttpClient : ISimpleHttpClient
    {
        private readonly HttpClient _httpClient;

        public SimpleHttpClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
        {
            return _httpClient.SendAsync(request);
        }

        public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return _httpClient.SendAsync(request, cancellationToken);
        }
    }
}