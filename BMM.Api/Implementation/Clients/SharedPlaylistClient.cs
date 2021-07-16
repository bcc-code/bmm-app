using System.Net.Http;
using System.Threading.Tasks;
using BMM.Api.Framework;
using BMM.Api.Framework.HTTP;
using BMM.Api.Implementation.Clients.Contracts;
using BMM.Api.Implementation.Models;
using Tavis.UriTemplates;

namespace BMM.Api.Implementation.Clients
{
    public class SharedPlaylistClient : BaseClient, ISharedPlaylistClient
    {
        private const string SharingSecret = "sharingSecret";

        public SharedPlaylistClient(IRequestHandler handler, ApiBaseUri baseUri, ILogger logger)
            : base(handler, baseUri, logger)
        {
        }

        public async Task<TrackCollection> Get(string sharingSecret)
        {
            var uri = new UriTemplate(ApiUris.SharedPlaylist);
            uri.SetParameter(SharingSecret, sharingSecret);
            return await Get<TrackCollection>(uri);
        }

        public async Task<bool> Follow(string sharingSecret)
        {
            var uri = new UriTemplate(ApiUris.SharedPlaylistFollow);
            uri.SetParameter(SharingSecret, sharingSecret);
            return await RequestIsSuccessful(BuildRequest(uri, HttpMethod.Post));
        }
    }
}