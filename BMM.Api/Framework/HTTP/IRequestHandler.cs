using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace BMM.Api.Framework.HTTP
{
    public interface IRequestHandler
    {
        /// <summary>
        ///     Gets the resolved response.
        /// </summary>
        /// <returns>The resolved response.</returns>
        /// <param name="request">Request.</param>
        /// <param name="cancellationToken"></param>
        Task<T> GetResolvedResponse<T>(
            IRequest request,
            IDictionary<string, string> customHeaders = default,
            CancellationToken? cancellationToken = null);

        /// <summary>
        ///     Gets the response.
        /// </summary>
        /// <returns>The response.</returns>
        /// <param name="request">Request.</param>
        /// <param name="cancellationToken"></param>
        Task<HttpResponseMessage> GetResponse(
            IRequest request,
            IDictionary<string, string> customHeaders = default,
            CancellationToken? cancellationToken = null);
    }
}