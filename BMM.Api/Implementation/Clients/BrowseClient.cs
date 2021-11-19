using System.Collections.Generic;
using System.Threading.Tasks;
using BMM.Api.Abstraction;
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

        public Task<IEnumerable<Document>> Get(CachePolicy policy)
        {
            var uri = new UriTemplate(ApiUris.Browse);
            return Get<IEnumerable<Document>>(uri);
        }

        public Task<GenericDocumentsHolder> GetDocuments(string path, int skip, int take)
        {
            string parameters = "{?skip,take}";
            var uri = new UriTemplate($"{path}{parameters}");
            uri.SetParameter("skip", skip);
            uri.SetParameter("take", take);

            return Get<GenericDocumentsHolder>(uri);
        }
    }
}