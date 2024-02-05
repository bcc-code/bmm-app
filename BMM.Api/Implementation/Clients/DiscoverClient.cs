using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BMM.Api.Abstraction;
using BMM.Api.Framework;
using BMM.Api.Framework.HTTP;
using BMM.Api.Implementation.Clients.Contracts;
using BMM.Api.Implementation.Models;
using BMM.Api.Implementation.Models.Enums;
using Tavis.UriTemplates;

namespace BMM.Api.Implementation.Clients
{
    public class DiscoverClient : BaseClient, IDiscoverClient
    {
        public DiscoverClient(IRequestHandler handler, ApiBaseUri baseUri, ILogger logger) : base(handler, baseUri, logger)
        { }

        public async Task<IEnumerable<Document>> GetDocuments(int? age, AppTheme theme, CachePolicy cachePolicy)
        {
            var uri = new UriTemplate(ApiUris.Discover);
            if (age.HasValue)
                uri.SetParameter("age", age.Value);
            uri.SetParameter("theme", theme);
            return FilterUnsupportedDocuments((await Get<IEnumerable<Document>>(uri)).ToList());
        }
    }
}