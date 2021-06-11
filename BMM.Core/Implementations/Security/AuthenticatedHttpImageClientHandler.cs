using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using BMM.Api.Abstraction;
using BMM.Api.Framework.Exceptions;

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
            foreach (var header in await _headerProvider.GetHeaders().ConfigureAwait(false))
            {
                request.Headers.Add(header.Key, header.Value);
            }

            try
            {
                return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
            }
            catch (FFImageLoading.Exceptions.DownloadAggregateException downloadAggregateException) when (downloadAggregateException.InnerException is HttpRequestException)
            {
                throw new InternetProblemsException(downloadAggregateException.InnerException);
            }
        }
    }
}