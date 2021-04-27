using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using BMM.Api.Abstraction;
using BMM.Api.Framework;
using BMM.Api.Framework.HTTP;
using BMM.Api.Implementation.Clients.Contracts;
using BMM.Api.Implementation.Models;
using Tavis.UriTemplates;

namespace BMM.Api.Implementation.Clients
{
    public class PodcastClient : BaseClient, IPodcastClient
    {
        public PodcastClient(IRequestHandler handler, ApiBaseUri baseUri, ILogger logger) : base(handler, baseUri, logger)
        { }

        public Task<IList<Podcast>> GetAll(CachePolicy cachePolicy)
        {
            var uri = new UriTemplate(ApiUris.Podcasts);
            return Get<IList<Podcast>>(uri);
        }

        public Task<Podcast> GetById(int id, CachePolicy cachePolicy)
        {
            var uri = new UriTemplate(ApiUris.Podcast);
            uri.SetParameter("id", id);

            return Get<Podcast>(uri);
        }

        public async Task<Stream> GetCover(int podcastId)
        {
            return await GetCoverBase(podcastId, ApiUris.PodcastCover);
        }

        public Task<IList<Track>> GetTracks(int podcastId, CachePolicy cachePolicy, int size = ApiConstants.LoadMoreSize, int from = 0)
        {
            var uri = new UriTemplate(ApiUris.PodcastTracks);
            uri.SetParameter("id", podcastId);
            uri.SetParameter("size", size);
            uri.SetParameter("from", from);

            return Get<IList<Track>>(uri);
        }

        public Task<Track> GetRandomTrack(int podcastId)
        {
            var uri = new UriTemplate(ApiUris.PodcastRandom);
            uri.SetParameter("id", podcastId);

            return Get<Track>(uri);
        }
    }
}