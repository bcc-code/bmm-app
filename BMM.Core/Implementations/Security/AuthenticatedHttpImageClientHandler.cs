using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using BMM.Api.Abstraction;

namespace BMM.Core.Implementations.Security
{
    public class AuthenticatedHttpImageClientHandler : HttpClientHandler
    {
        private readonly IMediaRequestHttpHeaders _headerProvider;

        public AuthenticatedHttpImageClientHandler(IMediaRequestHttpHeaders headerProvider)
        {
            _headerProvider = headerProvider;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            foreach (var header in await _headerProvider.GetHeaders())
            {
                request.Headers.Add(header.Key, header.Value);
            }

            return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
        }
    }
}