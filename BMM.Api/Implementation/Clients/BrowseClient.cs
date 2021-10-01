using System.Collections.Generic;
using System.Threading.Tasks;
using BMM.Api.Framework;
using BMM.Api.Framework.HTTP;
using BMM.Api.Implementation.Clients.Contracts;
using BMM.Api.Implementation.Models;
using Tavis.UriTemplates;

namespace BMM.Api.Implementation.Clients
{
    public class BrowseClient : BaseClient, IBrowseClient
    {
        public BrowseClient(IRequestHandler handler, ApiBaseUri baseUri, ILogger logger) : base(handler, baseUri, logger)
        {
        }

        public Task<IEnumerable<Document>> Get()
        {
            var uri = new UriTemplate(ApiUris.Browse);
            return Get<IEnumerable<Document>>(uri);
        }
    }
}