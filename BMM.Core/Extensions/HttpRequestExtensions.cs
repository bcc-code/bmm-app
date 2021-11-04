using System.Net.Http;
using System.Threading.Tasks;
using BMM.Api.Abstraction;

namespace BMM.Core.Extensions
{
    public static class HttpRequestExtensions
    {
        public static async Task AddHeaders(this HttpRequestMessage httpRequestMessage, IMediaRequestHttpHeaders headersProvider)
        {
            foreach (var header in await headersProvider.GetHeaders().ConfigureAwait(false))
                httpRequestMessage.Headers.Add(header.Key, header.Value);
        }
    }
}