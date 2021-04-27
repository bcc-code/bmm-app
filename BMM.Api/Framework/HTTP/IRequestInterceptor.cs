using System.Threading.Tasks;

namespace BMM.Api.Framework.HTTP
{
    public interface IRequestInterceptor
    {
        Task InterceptRequest(IRequest request);
    }
}