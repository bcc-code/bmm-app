using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using BMM.Api.Abstraction;
using BMM.Api.Framework.Exceptions;
using BMM.Core.Extensions;
using Xamarin.Android.Net;

namespace BMM.UI.Droid.Application.Networking
{
    public class DroidAuthenticatedNativeHttpImageClientHandler : AndroidClientHandler
    {
        private readonly IMediaRequestHttpHeaders _headerProvider;

        public DroidAuthenticatedNativeHttpImageClientHandler(IMediaRequestHttpHeaders headerProvider)
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