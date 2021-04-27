using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace BMM.Api.Framework.HTTP
{
    public interface ISimpleHttpClient
    {
        Task<HttpResponseMessage> SendAsync(HttpRequestMessage request);

        Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken);
    }
}