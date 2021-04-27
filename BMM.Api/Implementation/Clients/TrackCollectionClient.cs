using System.Collections.Generic;
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
    public class TrackCollectionClient : BaseClient, ITrackCollectionClient
    {
        public TrackCollectionClient(IRequestHandler handler, ApiBaseUri baseUri, ILogger logger)
            : base(handler, baseUri, logger)
        { }

        public async Task AddAlbumToTrackCollection(int id, int albumId)
        {
            var uri = new UriTemplate(ApiUris.TrackCollectionAlbum);
            uri.SetParameter("id", id);
            uri.SetParameter("albumId", albumId);

            var request = BuildRequest(uri, HttpMethod.Post);
            await RequestIsSuccessful(request);
        }

        public async Task<bool> AddTracksToTrackCollection(int id, IList<int> trackIds)
        {
            var uri = new UriTemplate(ApiUris.TrackCollection);
            uri.SetParameter("id", id);

            var request = BuildRequest(uri, HttpMethod.Post);
            request.Headers["X-HTTP-METHOD-OVERRIDE"] = "LINK";
            request.Headers["Link"] = string.Join(",", trackIds.Select(trackId => "</track/" + trackId + ">").ToList());

            return await RequestIsSuccessful(request);
        }

        public async Task<bool> Delete(int id)
        {
            var uri = new UriTemplate(ApiUris.TrackCollection);
            uri.SetParameter("id", id);

            var request = BuildRequest(uri, HttpMethod.Delete);

            return await TryRequestIsSuccessful(request);
        }

        public Task<IList<TrackCollection>> GetAll(CachePolicy cachePolicy)
        {
            var uri = new UriTemplate(ApiUris.TrackCollections);
            return Get<IList<TrackCollection>>(uri);
        }

        public Task<TrackCollection> GetById(int id, CachePolicy cachePolicy)
        {
            var uri = new UriTemplate(ApiUris.TrackCollection);
            uri.SetParameter("id", id);

            return Get<TrackCollection>(uri);
        }

        public Task Save(TrackCollection collection)
        {
            var uri = new UriTemplate(ApiUris.TrackCollection);
            uri.SetParameter("id", collection.Id);

            var reference = new TrackCollectionReference(collection);
            IRequest request = BuildRequest(uri, HttpMethod.Put, reference);

            return RequestHandler.GetResponse(request);
        }

        public async Task<int> Create(TrackCollection collection)
        {
            var uri = new UriTemplate(ApiUris.TrackCollection);
            var reference = new TrackCollectionReference(collection);
            IRequest request = BuildRequest(uri, HttpMethod.Post, reference);

            using (HttpResponseMessage response = await RequestHandler.GetResponse(request))
            {
                // ReSharper disable once AssignNullToNotNullAttribute
                // If response would be null it would have already thrown an exception
                return int.Parse(response.Headers.GetValues("X-Document-Id").FirstOrDefault());
            }
        }
    }
}