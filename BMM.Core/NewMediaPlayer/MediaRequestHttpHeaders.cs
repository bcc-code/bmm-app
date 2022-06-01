using System.Collections.Generic;
using System.Threading.Tasks;
using BMM.Api.Abstraction;
using BMM.Core.Implementations.ApiClients;

namespace BMM.Core.NewMediaPlayer
{
    public class MediaRequestHttpHeaders : IMediaRequestHttpHeaders
    {
        private readonly HttpHeaderProviders.MediaRequests _mediaRequestHeaders;

        public MediaRequestHttpHeaders(HttpHeaderProviders.MediaRequests mediaRequestHeaders)
        {
            _mediaRequestHeaders = mediaRequestHeaders;
        }

        public async Task<IList<KeyValuePair<string, string>>> GetHeaders()
        {
            var headers = new List<KeyValuePair<string, string>>();
            foreach (var headerProvider in HeaderProviders)
            {
                var header = await headerProvider.GetHeader();
                if (header.HasValue)
                    headers.Add(header.Value);
            }

            return headers;
        }

        public IList<IHeaderProvider> HeaderProviders => _mediaRequestHeaders.GetProviders();
    }
}