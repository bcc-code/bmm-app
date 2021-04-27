using System.Collections.Generic;
using System.Threading.Tasks;
using BMM.Api.Framework;
using BMM.Api.Framework.HTTP;
using BMM.Api.Implementation.Clients.Contracts;
using BMM.Api.Implementation.Models;
using Tavis.UriTemplates;

namespace BMM.Api.Implementation.Clients
{
    public class FacetsClient : BaseClient, IFacetsClient
    {
        public FacetsClient(IRequestHandler handler, ApiBaseUri baseUri, ILogger logger) : base(handler, baseUri, logger)
        { }

        public Task<IList<DocumentYear>> GetAlbumPublishedYears()
        {
            var uri = new UriTemplate(ApiUris.FacetsAlbumPublishedYears);
            return Get<IList<DocumentYear>>(uri);
        }
    }
}