using Android.Content;
using BMM.Api.Abstraction;
using BMM.Core.Implementations.Security;
using BMM.Core.Implementations.UI;
using Com.Google.Android.Exoplayer2;
using Com.Google.Android.Exoplayer2.Drm;
using Com.Google.Android.Exoplayer2.Source;
using Com.Google.Android.Exoplayer2.Upstream;
using Com.Google.Android.Exoplayer2.Util;
using MvvmCross;

namespace BMM.UI.Droid.Application.NewMediaPlayer.Playback
{
    public class SingleMediaSourceFactory
        : Java.Lang.Object,
          IMediaSource.IFactory
    {
        private readonly Context _applicationContext;
        private readonly IMediaRequestHttpHeaders _mediaRequestHeaders;
        private readonly IAccessTokenProvider _accessTokenProvider;

        public SingleMediaSourceFactory(Context applicationContext,
            IMediaRequestHttpHeaders mediaRequestHeaders,
            IAccessTokenProvider accessTokenProvider)
        {
            _mediaRequestHeaders = mediaRequestHeaders;
            _accessTokenProvider = accessTokenProvider;
            _applicationContext = applicationContext;
        }

        public IMediaSource CreateMediaSource(MediaItem mediaItem)
        {
            IDataSource.IFactory sourceFactory;
            
            var localPath = mediaItem!.MediaMetadata!.Extras!.GetString(MetadataMapper.MetadataKeyLocalPath);
            if (localPath != null)
            {
                sourceFactory = new FileDataSource.Factory();
            }
            else
            {
                var userAgent = Util.GetUserAgent(_applicationContext, "BMM Android");
                var bandwidthMeter = new DefaultBandwidthMeter.Builder(_applicationContext).Build();
                var dataSourceFactory = new DefaultHttpDataSource.Factory();

                dataSourceFactory.SetUserAgent(userAgent);
                dataSourceFactory.SetTransferListener(bandwidthMeter);
                dataSourceFactory.SetConnectTimeoutMs(DefaultHttpDataSource.DefaultConnectTimeoutMillis);
                dataSourceFactory.SetReadTimeoutMs(DefaultHttpDataSource.DefaultReadTimeoutMillis);
                dataSourceFactory.SetAllowCrossProtocolRedirects(true);

                sourceFactory = new HttpSourceFactory(
                    dataSourceFactory,
                    _mediaRequestHeaders,
                    _accessTokenProvider);
            }

            var extractorFactory = new ProgressiveMediaSource.Factory(sourceFactory);
            return extractorFactory.CreateMediaSource(mediaItem);
        }

        public int[] GetSupportedTypes()
        {
            return new[]
            {
                C.AudioContentTypeSpeech,
                C.AudioContentTypeMusic
            };
        }

        public IMediaSource.IFactory SetDrmSessionManagerProvider(IDrmSessionManagerProvider drmSessionManagerProvider) => this;
        public IMediaSource.IFactory SetLoadErrorHandlingPolicy(ILoadErrorHandlingPolicy loadErrorHandlingPolicy) => this;
    }
}