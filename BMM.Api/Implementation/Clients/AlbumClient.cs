using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BMM.Api.Framework;
using BMM.Api.Framework.HTTP;
using BMM.Api.Implementation.Clients.Contracts;
using BMM.Api.Implementation.Models;
using Tavis.UriTemplates;

namespace BMM.Api.Implementation.Clients
{
    public class AlbumClient : BaseClient, IAlbumClient
    {
        public AlbumClient(IRequestHandler handler, ApiBaseUri baseUri, ILogger logger)
            : base(handler, baseUri, logger)
        { }

        public Task<IList<Album>> GetAll(int size = ApiConstants.LoadMoreSize, int from = 0, IEnumerable<TrackSubType> contentTypes = null,
            IEnumerable<string> tags = null, IEnumerable<string> excludeTags = null)
        {
            var uri = new UriTemplate(ApiUris.Albums);
            uri.SetParameter("size", size);
            uri.SetParameter("from", from);

            if (contentTypes != null)
            {
                uri.SetParameter("content%2Dtype[]", contentTypes.Select(x => x.ToString().ToLower()));
            }

            if (tags != null)
            {
                uri.SetParameter("tags[]", tags);
            }

            if (excludeTags != null)
            {
                uri.SetParameter("exclude-tags[]", excludeTags);
            }

            return Get<IList<Album>>(uri);
        }

        public Task<Album> GetById(int id)
        {
            var uri = new UriTemplate(ApiUris.Album);
            uri.SetParameter("id", id);

            return Get<Album>(uri);
        }

        public async Task<Stream> GetCover(int id)
        {
            return await GetCoverBase(id, ApiUris.AlbumCover);
        }

        public Task<IList<Album>> GetPublishedByYear(int year)
        {
            var uri = new UriTemplate(ApiUris.AlbumsByPublishedYear);
            uri.SetParameter("year", year);

            return Get<IList<Album>>(uri);
        }

        public Task<IList<Album>> GetRecordedByYear(int year)
        {
            var uri = new UriTemplate(ApiUris.AlbumsByRecordedYear);
            uri.SetParameter("year", year);

            return Get<IList<Album>>(uri);
        }
    }
}