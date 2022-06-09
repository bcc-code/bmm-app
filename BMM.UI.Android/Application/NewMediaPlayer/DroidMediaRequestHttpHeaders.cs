using System.Collections.Generic;
using System.Threading.Tasks;
using BMM.Api.Abstraction;
using BMM.Core.Implementations.ApiClients;

namespace BMM.UI.Droid.Application.NewMediaPlayer
{
    public class DroidMediaRequestHttpHeaders : IMediaRequestHttpHeaders
    {
        private readonly HttpHeaderProviders.AndroidMediaRequests _mediaRequestHeaders;

        public DroidMediaRequestHttpHeaders(HttpHeaderProviders.AndroidMediaRequests mediaRequestHeaders)
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
                if (header.HasValue)
                    headers.Add(header.Value);
            }

            return headers;
        }
    }
}