using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using BMM.Api.Abstraction;
using BMM.Api.Framework;
using BMM.Api.Framework.HTTP;
using BMM.Api.Implementation;
using BMM.Api.Implementation.Clients.Contracts;
using BMM.Api.Implementation.Models;
using Tavis.UriTemplates;

namespace BMM.Api
{
    public class PlaylistClient : BaseClient, IPlaylistClient
    {
        public PlaylistClient(IRequestHandler handler, ApiBaseUri baseUri, ILogger logger) : base(handler, baseUri, logger)
        {
        }

        public Task<IList<Playlist>> GetAll(CachePolicy cachePolicy)
        {
            var uri = new UriTemplate(ApiUris.Playlists);
            return Get<IList<Playlist>>(uri);
        }

        public Task<Playlist> GetById(int id, CachePolicy cachePolicy)
        {
            var uri = new UriTemplate(ApiUris.Playlist);
            uri.SetParameter("id", id);

            return Get<Playlist>(uri);
        }

        public async Task<Stream> GetCover(int podcastId)
        {
            return await GetCoverBase(podcastId, ApiUris.PlaylistCover);
        }

        public Task<GenericDocumentsHolder> GetDocuments(int? age, CachePolicy cachePolicy)
        {
            var uri = new UriTemplate(ApiUris.PlaylistDocuments);
            uri.SetParameter("age", age);
            return Get<GenericDocumentsHolder>(uri);
        }

        // todo this times often out because it is so slow
        public Task<IList<Track>> GetTracks(int podcastId, CachePolicy cachePolicy)
        {
            var uri = new UriTemplate(ApiUris.PlaylistTracks);
            uri.SetParameter("id", podcastId);

            return Get<IList<Track>>(uri);
        }
    }
}
