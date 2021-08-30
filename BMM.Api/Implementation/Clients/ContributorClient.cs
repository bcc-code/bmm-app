using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using BMM.Api.Abstraction;
using BMM.Api.Framework;
using BMM.Api.Framework.HTTP;
using BMM.Api.Implementation.Clients.Contracts;
using BMM.Api.Implementation.Models;
using Tavis.UriTemplates;

namespace BMM.Api.Implementation.Clients
{
    public class ContributorClient : BaseClient, IContributorClient
    {
        public ContributorClient(IRequestHandler handler, ApiBaseUri baseUri, ILogger logger)
            : base(handler, baseUri, logger)
        { }

        public async Task<int> Add(Contributor contributor)
        {
            var uri = new UriTemplate(ApiUris.Contributor);

            var request = BuildRequest(uri, HttpMethod.Post, contributor);
            using (HttpResponseMessage response = await RequestHandler.GetResponse(request))
            {
                if (response == null)
                    return 0;

                return int.Parse(response.Headers.GetValues("X-Document-Id").FirstOrDefault());
            }
        }

        public Task<IList<Contributor>> GetAll(int size = ApiConstants.LoadMoreSize, int from = 0, string orderBy = null)
        {
            var uri = new UriTemplate(ApiUris.Contributors);
            uri.SetParameter("size", size);
            uri.SetParameter("from", from);

            if (orderBy != null)
            {
                uri.SetParameter("orderby", orderBy);
            }

            return Get<IList<Contributor>>(uri);
        }

        public Task<Contributor> GetById(int id)
        {
            var uri = new UriTemplate(ApiUris.Contributor);
            uri.SetParameter("id", id);

            return Get<Contributor>(uri);
        }

        public Task<IList<Contributor>> GetByTerm(string term, int size = ApiConstants.LoadMoreSize)
        {
            var uri = new UriTemplate(ApiUris.ContributorsByName);
            uri.SetParameter("term", term);
            uri.SetParameter("size", size);

            return Get<IList<Contributor>>(uri);
        }

        public Task<Stream> GetCover(int contributorId)
        {
            return GetCoverBase(contributorId, ApiUris.ContributorCover);
        }

        public Task<IList<Track>> GetTracks(int contributorId,
            CachePolicy cachePolicy,
            int size = ApiConstants.LoadMoreSize,
            int @from = 0,
            IEnumerable<string> contentTypes = null)
        {
            var uri = new UriTemplate(ApiUris.ContributorTracks);
            uri.SetParameter("id", contributorId);
            uri.SetParameter("size", size);
            uri.SetParameter("from", from);

            if (contentTypes != null)
            {
                uri.SetParameter("content%2Dtype[]", contentTypes);
            }

            return Get<IList<Track>>(uri);
        }
    }
}