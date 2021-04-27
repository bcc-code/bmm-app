using System.Threading.Tasks;
using BMM.Api.Framework;
using BMM.Api.Framework.HTTP;
using BMM.Api.Implementation.Clients.Contracts;
using BMM.Api.Implementation.Models;
using Tavis.UriTemplates;

namespace BMM.Api.Implementation.Clients
{
    public class ApiInfoClient : BaseClient, IApiInfoClient
    {
        public ApiInfoClient(IRequestHandler handler, ApiBaseUri baseUri, ILogger logger) : base(handler, baseUri, logger)
        { }

        public Task<ApiInfo> GetInfo()
        {
            var uri = new UriTemplate(ApiUris.ApiRoot);
            return Get<ApiInfo>(uri);
        }
    }
}