using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BMM.Api.Framework;
using BMM.Api.Framework.HTTP;
using BMM.Api.Implementation.Clients.Contracts;
using BMM.Api.Implementation.Models;
using Tavis.UriTemplates;

namespace BMM.Api.Implementation.Clients
{
    public class SearchClient : BaseClient, ISearchClient
    {
        public SearchClient(IRequestHandler handler, ApiBaseUri baseUri, ILogger logger)
            : base(handler, baseUri, logger)
        { }

        public async Task<IList<Document>> GetAll(string term, int size = ApiConstants.LoadMoreSize, int from = 0, IEnumerable<string> resourceTypes = null,
            IEnumerable<string> contentTypes = null, DateTime? datetimeFrom = null, DateTime? datetimeTo = null, IEnumerable<string> tags = null,
            IEnumerable<string> excludeTags = null, UnpublishedEnum? unpublished = null)
        {
            var uri = new UriTemplate(ApiUris.Search);
            uri.SetParameter("term", term);
            uri.SetParameter("size", size);
            uri.SetParameter("from", from);

            if (resourceTypes != null)
            {
                uri.SetParameter("resource%2Dtype[]", resourceTypes);
            }

            if (contentTypes != null)
            {
                uri.SetParameter("content%2Dtype[]", contentTypes);
            }

            if (datetimeFrom != null)
            {
                uri.SetParameter("datetime%2Dfrom", datetimeFrom);
            }

            if (datetimeTo != null)
            {
                uri.SetParameter("datetime%2Dto", datetimeTo);
            }

            if (tags != null)
            {
                uri.SetParameter("tags[]", tags);
            }

            if (excludeTags != null)
            {
                uri.SetParameter("exclude-tags[]", excludeTags);
            }

            if (unpublished != null)
            {
                uri.SetParameter("unpublished", unpublished.ToString().ToLower());
            }

            return FilterUnsupportedDocuments(await Get<IList<Document>>(uri)).ToList();
        }

        public Task<IList<string>> GetSuggestions(string term)
        {
            var uri = new UriTemplate(ApiUris.Suggestions);
            uri.SetParameter("term", term);

            return Get<IList<string>>(uri);
        }
    }
}