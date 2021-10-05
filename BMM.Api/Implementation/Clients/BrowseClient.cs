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

        public Task<IEnumerable<Document>> GetEvents(int skip, int take)
        {
            var uri = new UriTemplate(ApiUris.BrowseEvents);
            uri.SetParameter("skip", skip);
            uri.SetParameter("take", take);

            return Get<IEnumerable<Document>>(uri);
        }

        public Task<IEnumerable<Document>> GetAudiobooks(int skip, int take)
        {
            var uri = new UriTemplate(ApiUris.BrowseAudiobooks);
            uri.SetParameter("skip", skip);
            uri.SetParameter("take", take);

            return Get<IEnumerable<Document>>(uri);
        }

        public Task<IEnumerable<Document>> GetMusic()
        {
            var uri = new UriTemplate(ApiUris.BrowseMusic);
            return Get<IEnumerable<Document>>(uri);
        }

        public Task<IEnumerable<Document>> GetPodcasts()
        {
            var uri = new UriTemplate(ApiUris.BrowsePodcasts);
            return Get<IEnumerable<Document>>(uri);
        }
    }
}