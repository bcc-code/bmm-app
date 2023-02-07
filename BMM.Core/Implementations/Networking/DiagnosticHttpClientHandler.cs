using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace BMM.Core.Implementations.Networking
{
    public class DiagnosticsClientHandler : HttpClientHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Debug.WriteLine($"[{request.Method}] {request.RequestUri}");
            if (request.Content != null)
            {
                var requestContent = await request.Content.ReadAsStringAsync();
                if (requestContent != null)
                    Debug.WriteLine(requestContent);
            }

            var response = await base.SendAsync(request, cancellationToken);
            if (response.Content != null)
            {
                var content = await response.Content?.ReadAsStringAsync();
                if (content != null)
                    Debug.WriteLine(content);
            }

            return response;
        }
    }
}