using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BMM.Api.Framework;
using BMM.Api.Framework.HTTP;
using BMM.Api.Implementation.Clients.Contracts;
using BMM.Api.Implementation.Models;
using BMM.Api.Implementation.Models.Enums;
using Tavis.UriTemplates;

namespace BMM.Api.Implementation.Clients
{
    public class SearchClient : BaseClient, ISearchClient
    {
        public SearchClient(IRequestHandler handler, ApiBaseUri baseUri, ILogger logger)
            : base(handler, baseUri, logger)
        { }

        public async Task<SearchResults> GetAll(string term, SearchFilter searchFilter = SearchFilter.All, int from = 0, int size = ApiConstants.LoadMoreSize)
        {
            var uri = new UriTemplate(ApiUris.Search);
            uri.SetParameter("term", term);
            uri.SetParameter("filter", searchFilter);
            uri.SetParameter("size", size);
            uri.SetParameter("from", from);

            var results = await Get<SearchResults>(uri);
            results.Items = FilterUnsupportedDocuments(results.Items.ToList());
            return results;
        }

        public Task<IList<string>> GetSuggestions(string term)
        {
            var uri = new UriTemplate(ApiUris.Suggestions);
            uri.SetParameter("term", term);

            return Get<IList<string>>(uri);
        }
    }
}