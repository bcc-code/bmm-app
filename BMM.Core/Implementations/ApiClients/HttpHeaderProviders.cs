using System.Collections.Generic;
using BMM.Api.Abstraction;
using BMM.Api.RequestInterceptor;
using BMM.Core.Implementations.Networking;
using BMM.Core.NewMediaPlayer;
using MvvmCross;

namespace BMM.Core.Implementations.ApiClients
{
    public interface IPredefinedHeaderSet
    {
        IList<IHeaderProvider> GetProviders();
    }

    public class HttpHeaderProviders
    {
        public class UnauthorizedRequests : IPredefinedHeaderSet
        {
            public IList<IHeaderProvider> GetProviders()
            {
                return new List<IHeaderProvider>
                {
                    Mvx.IoCProvider.Resolve<ContentLanguageHeaderProvider>(),
                    Mvx.IoCProvider.Resolve<JsonContentTypeHeaderProvider>(),
                    Mvx.IoCProvider.Resolve<BmmVersionHeaderProvider>(),
                    Mvx.IoCProvider.Resolve<ExperimentIdHeaderProvider>()
                };
            }
        }

        public class AuthorizedRequests : IPredefinedHeaderSet
        {
            public IList<IHeaderProvider> GetProviders()
            {
                return new List<IHeaderProvider>
                {
                    Mvx.IoCProvider.Resolve<ContentLanguageHeaderProvider>(),
                    Mvx.IoCProvider.Resolve<JsonContentTypeHeaderProvider>(),
                    Mvx.IoCProvider.Resolve<BmmVersionHeaderProvider>(),
                    Mvx.IoCProvider.Resolve<ExperimentIdHeaderProvider>(),
                    Mvx.IoCProvider.Resolve<ConnectivityHeaderProvider>(),
                    Mvx.IoCProvider.Resolve<MobileDownloadAllowedHeaderProvider>(),
                    Mvx.IoCProvider.Resolve<IAuthorizationHeaderProvider>()
                };
            }
        }

        public class MediaRequests : IPredefinedHeaderSet
        {
            public IList<IHeaderProvider> GetProviders()
            {
                return new List<IHeaderProvider>
                {
                    Mvx.IoCProvider.Resolve<ConnectivityHeaderProvider>(),
                    Mvx.IoCProvider.Resolve<MobileDownloadAllowedHeaderProvider>(),
                    Mvx.IoCProvider.Resolve<IAuthorizationHeaderProvider>()
                };
            }
        }
        
        public class AndroidMediaRequests : IPredefinedHeaderSet
        {
            public IList<IHeaderProvider> GetProviders()
            {
                return new List<IHeaderProvider>
                {
                    Mvx.IoCProvider.Resolve<ConnectivityHeaderProvider>(),
                    Mvx.IoCProvider.Resolve<MobileDownloadAllowedHeaderProvider>(),
                    Mvx.IoCProvider.Resolve<ISyncAuthorizationHeaderProvider>()
                };
            }
        }
    }
}
