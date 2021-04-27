using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using BMM.Api.Framework;
using BMM.Api.Framework.HTTP;
using BMM.Api.Implementation.Clients.Contracts;
using Tavis.UriTemplates;

namespace BMM.Api.Implementation.Clients
{
    public class FileClient : BaseClient, IFileClient
    {
        public FileClient(IRequestHandler handler, ApiBaseUri baseUri, ILogger logger)
            : base(handler, baseUri, logger)
        { }

        public async Task<Stream> GetFile(string uri)
        {
            if (!uri.StartsWith(BaseUri.AbsoluteUri + "file/protected/"))
            {
                throw new ArgumentException($"The URI '{uri}' is not allowed in FileClient.");
            }

            var request = BuildRequest(new UriTemplate(uri), HttpMethod.Get);

            // We don't use using for this since we want to access the content stream even after the request has been disposed
            HttpResponseMessage response = await RequestHandler.GetResponse(request);
            return await response.Content.ReadAsStreamAsync();
        }
    }
}