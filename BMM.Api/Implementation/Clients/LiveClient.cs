using System;
using System.Threading.Tasks;
using BMM.Api.Framework;
using BMM.Api.Framework.HTTP;
using BMM.Api.Implementation.Clients.Contracts;
using BMM.Api.Implementation.Models;
using Tavis.UriTemplates;

namespace BMM.Api.Implementation.Clients
{
    public class LiveClient : BaseClient, ILiveClient
    {
        public LiveClient(IRequestHandler handler, ApiBaseUri baseUri, ILogger logger) : base(handler, baseUri, logger)
        { }

        public async Task<LiveInfo> GetInfo()
        {
            var uri = new UriTemplate(ApiUris.Live);
            var info = await Get<LiveInfo>(uri);
            info.LocalTime = DateTime.Now;
            return info;
        }
    }
}