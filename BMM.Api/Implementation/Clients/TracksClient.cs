using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using BMM.Api.Abstraction;
using BMM.Api.Framework;
using BMM.Api.Framework.HTTP;
using BMM.Api.Implementation.Clients.Contracts;
using BMM.Api.Implementation.Constants;
using BMM.Api.Implementation.Models;
using Tavis.UriTemplates;

namespace BMM.Api.Implementation.Clients
{
    public class TracksClient : BaseClient, ITracksClient
    {
        public TracksClient(IRequestHandler handler, ApiBaseUri baseUri, ILogger logger)
            : base(handler, baseUri, logger)
        { }

        public async Task<int> Add(TrackRaw track)
        {
            var uri = new UriTemplate(ApiUris.Tracks);

            var request = BuildRequest(uri, HttpMethod.Post, track);

            using (HttpResponseMessage response = await RequestHandler.GetResponse(request))
            {
                if (response == null)
                    return 0;

                return int.Parse(response.Headers.GetValues("X-Document-Id").FirstOrDefault());
            }
        }

        public Task<IList<Track>> GetAll(CachePolicy cachePolicy, int size = ApiConstants.LoadMoreSize, int @from = 0, IEnumerable<TrackSubType> contentTypes = null,
            IEnumerable<string> tags = null, IEnumerable<string> excludeTags = null)
        {
            var uri = new UriTemplate(ApiUris.Tracks);
            uri.SetParameter("size", size);
            uri.SetParameter("from", from);

            if (contentTypes != null)
            {
                uri.SetParameter("content%2Dtype[]", contentTypes.Select(x => x.ToString().ToLower()).ToArray());
            }

            if (tags != null)
            {
                uri.SetParameter("tags[]", tags);
            }

            if (excludeTags != null)
            {
                uri.SetParameter("exclude%2Dtags[]", excludeTags);
            }

            return Get<IList<Track>>(uri);
        }

        public Task<Track> GetById(int id, string desiredLanguage = default)
        {
            var uri = new UriTemplate(ApiUris.Track);
            uri.SetParameter("id", id);

            if (string.IsNullOrEmpty(desiredLanguage))
                return Get<Track>(uri);
            
            var headers = new Dictionary<string, string>
            {
                { HeaderNames.AcceptLanguage, desiredLanguage }
            };

            return Get<Track>(uri, headers);
        }

        public Task<IList<Track>> GetRecommendations()
        {
            var uri = new UriTemplate(ApiUris.TrackRecommendation);
            return Get<IList<Track>>(uri);
        }

        public Task<IList<Transcription>> GetTranscriptions(int trackId)
        {
            var uri = new UriTemplate(ApiUris.TrackTranscriptions);
            uri.SetParameter("id", trackId);
            return Get<IList<Transcription>>(uri);
        }

        public Task<Stream> GetCover(int id)
        {
            return GetCoverBase(id, ApiUris.TrackCover);
        }

        public Task<TrackRaw> GetRawById(int id)
        {
            var uri = new UriTemplate(ApiUris.Track);
            uri.SetParameter("id", id);
            uri.SetParameter("raw", 1);

            return Get<TrackRaw>(uri);
        }

        public Task<IList<Track>> GetRelated(TrackRelation relation,
            int size = 20,
            int from = 0,
            IEnumerable<string> contentTypes = null)
        {
            var uri = new UriTemplate(ApiUris.TracksRelated);
            uri.SetParameter("key", relation.Type.ToString().ToLower());
            uri.SetParameter("size", size);
            uri.SetParameter("from", from);

            // TODO: Finish the query by adding the different relation-specific parameters

            if (contentTypes != null)
            {
                uri.SetParameter("content%2Dtype[]", contentTypes);
            }

            return Get<IList<Track>>(uri);
        }

        public Task<bool> Save(TrackRaw track)
        {
            var uri = new UriTemplate(ApiUris.Track);
            uri.SetParameter("id", track.Id);

            var request = BuildRequest(uri, HttpMethod.Put, track);

            return TryRequestIsSuccessful(request);
        }

        public Task<bool> SetCover(int id, Stream file, string filename)
        {
            var uri = new UriTemplate(ApiUris.TrackCover);
            uri.SetParameter("id", id);

            var form = new MultipartFormDataContent
            {
                {new StreamContent(file), "file", filename}
            };

            var request = BuildRequest(uri, HttpMethod.Post, form);
            request.Headers.Add("X-HTTP-METHOD-OVERRIDE", "PUT");

            return TryRequestIsSuccessful(request);
        }
    }
}