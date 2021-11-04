using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using BMM.Api.Abstraction;
using BMM.Api.Framework.Exceptions;
using BMM.Core.Extensions;

namespace BMM.UI.iOS.Networking
{
    public class iOSAuthenticatedNativeHttpImageClientHandler : NSUrlSessionHandler
    {
        private readonly IMediaRequestHttpHeaders _headerProvider;

        public iOSAuthenticatedNativeHttpImageClientHandler(IMediaRequestHttpHeaders headerProvider)
        {
            _headerProvider = headerProvider;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            try
            {
                await request.AddHeaders(_headerProvider);
                return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
            }
            catch (FFImageLoading.Exceptions.DownloadAggregateException downloadAggregateException)
                when (downloadAggregateException.InnerException is HttpRequestException)
            {
                throw new InternetProblemsException(downloadAggregateException.InnerException);
            }
        }
    }
}