using System.Threading.Tasks;
using BMM.Api.Abstraction;
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
            
            Task.Run(_accessTokenProvider.UpdateAccessTokenIfNeeded).FireAndForget();
            var headers = _mediaRequestHttpHeaders.GetHeaders().Result;
            
            foreach (var header in headers)
                source.SetRequestProperty(header.Key, header.Value);
            
            return source;
        }
    }
}