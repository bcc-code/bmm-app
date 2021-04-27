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
            var providers = _mediaRequestHeaders.GetProviders();
            var headers = new List<KeyValuePair<string, string>>();
            foreach (var headerProvider in providers)
            {
                var header = await headerProvider.GetHeader();
                headers.Add(header);
            }

            return headers;
        }
    }
}