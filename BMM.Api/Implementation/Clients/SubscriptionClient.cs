using System.Net.Http;
using System.Threading.Tasks;
using BMM.Api.Framework;
using BMM.Api.Framework.HTTP;
using BMM.Api.Implementation.Clients.Contracts;
using BMM.Api.Implementation.Models;
using Tavis.UriTemplates;

namespace BMM.Api.Implementation.Clients
{
    public class SubscriptionClient : BaseClient, ISubscriptionClient
    {
        public SubscriptionClient(IRequestHandler handler, ApiBaseUri baseUri, ILogger logger)
            : base(handler, baseUri, logger)
        { }

        public Task<bool> Delete(Subscription subscription)
        {
            var uri = new UriTemplate(ApiUris.Notifications);
            uri.SetParameter("deviceId", subscription.DeviceId);

            var request = BuildRequest(uri, HttpMethod.Delete);

            return TryRequestIsSuccessful(request);
        }

        public Task<bool> Save(Subscription subscription)
        {
            var uri = new UriTemplate(ApiUris.Notifications);
            uri.SetParameter("deviceId", subscription.DeviceId);

            var request = BuildRequest(uri, HttpMethod.Put, subscription);

            return TryRequestIsSuccessful(request);
        }
    }
}