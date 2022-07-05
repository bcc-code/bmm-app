using System.Collections.Generic;
using System.Threading.Tasks;
using AVFoundation;
using BMM.Api.Abstraction;
using Foundation;

namespace BMM.UI.iOS.NewMediaPlayer
{
    public class AvPlayerItemFactory
    {
        private readonly IMediaRequestHttpHeaders _headers;

        public AvPlayerItemFactory(IMediaRequestHttpHeaders headers)
        {
            _headers = headers;
        }

        public async Task<BMMPlayerItem> Create(IMediaTrack mediaTrack)
        {
            var mediaUrl = MediaFileUrlHelper.GetUrlFor(mediaTrack);

            AVUrlAssetOptions options = null;
            if (TrackIsNotDownloaded(mediaTrack))
            {
                options = GetOptionsWithHeaders(await _headers.GetHeaders());
            }

            AVAsset asset = AVUrlAsset.Create(mediaUrl, options);

            var playerItem = new BMMPlayerItem(asset, mediaTrack);
            return playerItem;
        }

        private bool TrackIsNotDownloaded(IMediaTrack mediaTrack)
        {
            return mediaTrack.LocalPath == null;
        }

        private AVUrlAssetOptions GetOptionsWithHeaders(IList<KeyValuePair<string, string>> headers)
        {
            var headerDictionary = new NSMutableDictionary();
            foreach (var header in headers)
            {
                headerDictionary.Add(new NSString(header.Key), new NSString(header.Value));
            }

            var options = new AVUrlAssetOptions(NSDictionary.FromObjectAndKey(
                headerDictionary,
                (NSString)"AVURLAssetHTTPHeaderFieldsKey"
            ));

            return options;
        }
    }
}