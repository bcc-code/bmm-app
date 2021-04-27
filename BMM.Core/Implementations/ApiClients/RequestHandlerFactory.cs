using BMM.Api.Framework;
using BMM.Api.Framework.HTTP;
using BMM.Core.Implementations.Security;
using MvvmCross;

namespace BMM.Core.Implementations.ApiClients
{
    public class RequestHandlerFactory : IRequestHandlerFactory
    {
        public IRequestHandler BuildUnauthorizedRequestHandler()
        {
            var providers = Mvx.IoCProvider.Resolve<HttpHeaderProviders.UnauthorizedRequests>();
            return CreateInstance(new HeaderRequestInterceptor(providers.GetProviders()));
        }

        public IRequestHandler BuildRequestHandler()
        {
            var providers = Mvx.IoCProvider.Resolve<HttpHeaderProviders.AuthorizedRequests>();
            return CreateInstance(new HeaderRequestInterceptor(providers.GetProviders()));
        }

        public IRequestHandler CreateInstance(IRequestInterceptor requestInterceptor)
        {
            return new RequestHandler(Mvx.IoCProvider.Resolve<ISimpleHttpClient>(),
                Mvx.IoCProvider.Resolve<IConnection>(),
                requestInterceptor,
                Mvx.IoCProvider.Resolve<IBadRequestThrower>(),
                Mvx.IoCProvider.Resolve<IResponseDeserializer>());
        }
    }
}