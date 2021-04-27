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

        public async Task InterceptRequest(IRequest request)
        {
            foreach (var provider in _headerProviders)
            {
                var header = await provider.GetHeader();
                request.Headers.Add(header.Key, header.Value);
            }
        }
    }
}