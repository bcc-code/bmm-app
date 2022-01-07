using System.Collections.Generic;
using System.Threading.Tasks;
using BMM.Api.Framework.HTTP;

namespace BMM.Core.Implementations.ApiClients
{
    /// <summary>
    /// This class can be used to use multiple RequestInterceptors. It's not used anymore but might be helpful in the future.
    /// </summary>
    public class RequestInterceptorComposite : IRequestInterceptor
    {
        public IList<IRequestInterceptor> RequestInterceptors;

        public async Task InterceptRequest(IRequest request,  IDictionary<string, string> customHeaders = default)
        {
            // Do not run the test in parallel as all of them are accessing the request.header array. Consciously avoiding thread-safe collections.
            foreach (var interceptor in RequestInterceptors)
            {
                await interceptor.InterceptRequest(request);
            }
        }
    }
}