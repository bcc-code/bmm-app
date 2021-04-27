using Android.Content;
using Android.Net;
using Android.Support.V4.Media;
using BMM.Api.Abstraction;
using BMM.Api.Implementation.Models;
using Com.Google.Android.Exoplayer2.Ext.Mediasession;
using Com.Google.Android.Exoplayer2.Source;
using Com.Google.Android.Exoplayer2.Source.Hls;
using Com.Google.Android.Exoplayer2.Upstream;
using Com.Google.Android.Exoplayer2.Util;

namespace BMM.UI.Droid.Application.NewMediaPlayer.Playback
{
    public class SingleMediaSourceFactory : Java.Lang.Object, TimelineQueueEditor.IMediaSourceFactory
    {
        private readonly Context _applicationContext;
        private readonly IMediaRequestHttpHeaders _headers;

        public SingleMediaSourceFactory(Context applicationContext, IMediaRequestHttpHeaders headers)
        {
            _headers = headers;
            _applicationContext = applicationContext;
        }

        public IMediaSource CreateMediaSource(MediaDescriptionCompat description)
        {
            IDataSourceFactory sourceFactory;
            Uri uri;

            var localPath = description.Extras.GetString(MetadataMapper.MetadataKeyLocalPath);
            if (localPath != null)
            {
                uri = Uri.Parse(localPath);
                sourceFactory = new FileDataSource.Factory();
            }
            else
            {
                uri = description.MediaUri;
                var userAgent = Util.GetUserAgent(_applicationContext, "BMM Android");
                var bandwidthMeter = new DefaultBandwidthMeter.Builder(_applicationContext).Build();
                var dataSourceFactory = new DefaultHttpDataSourceFactory(userAgent, bandwidthMeter);

                // todo find a better solution to pass headers async
                var headers = _headers.GetHeaders().GetAwaiter().GetResult();
                sourceFactory = new HttpSourceFactory(dataSourceFactory, headers);

                var subtype = description.Extras.GetString(MetadataMapper.MetadataKeySubtype);
                if (subtype == TrackSubType.Live.ToString())
                {
                    TagReadingQueueNavigator.HackForDescriptionForStreamingTrack = description;
                    return new HlsMediaSource.Factory(dataSourceFactory)
                        .SetTag(description)
                        .SetAllowChunklessPreparation(true)
                        .CreateMediaSource(uri);
                }
            }

            var extractorFactory = new ProgressiveMediaSource.Factory(sourceFactory);

            // We need to set the tag so we can read it in <see cref="TagReadingQueueNavigator" />
            extractorFactory.SetTag(description);
            return extractorFactory.CreateMediaSource(uri);
        }
    }
}