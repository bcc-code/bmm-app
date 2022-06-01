using System.Linq;
using BMM.Api.Abstraction;
using BMM.Api.Implementation.Constants;
using BMM.Core.Extensions;
using BMM.Core.Implementations.Security;
using Com.Google.Android.Exoplayer2.Upstream;
using Object = Java.Lang.Object;

namespace BMM.UI.Droid.Application.NewMediaPlayer.Playback
{
    public class HttpSourceFactory : Object, IDataSourceFactory
    {
        private readonly DefaultHttpDataSourceFactory _httpFactory;
        private readonly IAccessTokenProvider _accessTokenProvider;
        private readonly IMediaRequestHttpHeaders _mediaRequestHttpHeaders;

        public HttpSourceFactory(
            DefaultHttpDataSourceFactory httpFactory,
            IMediaRequestHttpHeaders mediaRequestHttpHeaders,
            IAccessTokenProvider accessTokenProvider)
        {
            _httpFactory = httpFactory;
            _mediaRequestHttpHeaders = mediaRequestHttpHeaders;
            _accessTokenProvider = accessTokenProvider;
        }

        public IDataSource CreateDataSource()
        {
            if (!(_httpFactory.CreateDataSource() is DefaultHttpDataSource source))
                return null;
            
            if (!_accessTokenProvider.CheckIsTokenValid(_accessTokenProvider.AccessToken))
                _accessTokenProvider.GetAccessToken().FireAndForget();

            // Filtering out IAuthorizationHeaderProvider, because we need to avoid
            // waiting for refresh token operation here, as this is sync method.
            var headersProviders = _mediaRequestHttpHeaders
                .HeaderProviders
                .Where(hp => !(hp is IAuthorizationHeaderProvider));

            foreach (var headersProvider in headersProviders)
            {
                var header = headersProvider.GetHeader().Result;
                if (header == null)
                    continue;

                var headerKeyValuePair = header.Value;
                source.SetRequestProperty(headerKeyValuePair.Key, headerKeyValuePair.Value);
            }
            
            source.SetRequestProperty(HeaderNames.Authorization, $"Bearer {_accessTokenProvider.AccessToken}");
            return source;
        }
    }
}