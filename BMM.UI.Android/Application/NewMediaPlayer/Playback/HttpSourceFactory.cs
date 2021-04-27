using System.Collections.Generic;
using Com.Google.Android.Exoplayer2.Upstream;
using Object = Java.Lang.Object;

namespace BMM.UI.Droid.Application.NewMediaPlayer.Playback
{
    public class HttpSourceFactory : Object, IDataSourceFactory
    {
        private readonly DefaultHttpDataSourceFactory _httpFactory;
        private readonly IList<KeyValuePair<string, string>> _headers;

        public HttpSourceFactory(DefaultHttpDataSourceFactory httpFactory, IList<KeyValuePair<string, string>> headers)
        {
            _httpFactory = httpFactory;
            _headers = headers;
        }

        public IDataSource CreateDataSource()
        {
            var source = _httpFactory.CreateDataSource() as DefaultHttpDataSource;
            foreach (var header in _headers)
            {
                source?.SetRequestProperty(header.Key, header.Value);
            }

            return source;
        }
    }
}