using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using BMM.Api.Framework.HTTP;
using BMM.Api.Implementation;
using BMM.Api.Implementation.Models;
using Tavis.UriTemplates;

namespace BMM.Api.Framework
{
    public abstract class BaseClient : IClient
    {
        private readonly ILogger _logger;

        protected BaseClient(IRequestHandler handler, ApiBaseUri baseUri, ILogger logger)
        {
            _logger = logger;
            RequestHandler = handler;
            BaseUri = baseUri;
        }

        /// <summary>Gets the base URI of the resource server.</summary>
        public Uri BaseUri { get; }

        public IRequestHandler RequestHandler { get; }

        public IRequest BuildRequest(UriTemplate template, HttpMethod method, object body = null)
        {
            return new Request
            {
                Method = method,
                Uri = new Uri(BaseUri, template.Resolve()),
                Body = body
            };
        }

        protected IList<Document> FilterUnsupportedDocuments(IList<Document> documents)
        {
            var countBefore = documents.Count;
            var filtered = documents.Where(d => d.DocumentType != DocumentType.Unsupported).ToList();
            if (filtered.Count != countBefore)
                _logger.Warn(nameof(BaseClient), $"{countBefore - filtered.Count} unsupported documents filtered out");
            return filtered;
        }

        protected Task<T> Get<T>(UriTemplate uri, IDictionary<string, string> customHeaders = default)
        {
            var request = BuildRequest(uri, HttpMethod.Get);
            return RequestHandler.GetResolvedResponse<T>(request, customHeaders);
        }

        protected async Task<Stream> GetCoverBase(int id, string uriString)
        {
            try
            {
                var uri = new UriTemplate(uriString);
                uri.SetParameter("id", id);

                var request = BuildRequest(uri, HttpMethod.Get);

                using (HttpResponseMessage response = await RequestHandler.GetResponse(request))
                    return await response.Content.ReadAsStreamAsync();
            }
            catch
            {
                return Stream.Null;
            }
        }

        [Obsolete("Usages should be refactored to use the version which throws exceptions instead of swallowing it")]
        protected async Task<bool> TryRequestIsSuccessful(IRequest request)
        {
            try
            {
                using (HttpResponseMessage response = await RequestHandler.GetResponse(request))
                    return response.IsSuccessStatusCode;
            }
            catch
            {
                // ToDo: don't swallow exceptions
                return false;
            }
        }

        protected async Task<bool> RequestIsSuccessful(IRequest request)
        {
            using (HttpResponseMessage response = await RequestHandler.GetResponse(request))
                return response.IsSuccessStatusCode;
        }
    }
}