using System.Collections.Generic;
using System.Threading.Tasks;
using BMM.Api.Abstraction;
using BMM.Api.Framework.HTTP;

namespace BMM.Core.Implementations.Security
{
    public class HeaderRequestInterceptor : IRequestInterceptor
    {
        private readonly IList<IHeaderProvider> _headerProviders;

        public HeaderRequestInterceptor(IList<IHeaderProvider> headerProviders)
        {
            _headerProviders = headerProviders;
        }

        public async Task InterceptRequest(IRequest request, IDictionary<string, string> customHeaders)
        {
            foreach (var provider in _headerProviders)
            {
                var header = await provider.GetHeader();
                if (header.HasValue)
                    request.Headers.Add(header.Value.Key, header.Value.Value);
            }

            if (customHeaders == default)
                return;
            
            foreach (var customHeader in customHeaders)
                request.Headers[customHeader.Key] = customHeader.Value;
        }
    }
}