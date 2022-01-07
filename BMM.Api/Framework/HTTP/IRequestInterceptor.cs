using System.Collections.Generic;
using System.Threading.Tasks;

namespace BMM.Api.Framework.HTTP
{
    public interface IRequestInterceptor
    {
        Task InterceptRequest(IRequest request, IDictionary<string, string> customHeaders = default);
    }
}