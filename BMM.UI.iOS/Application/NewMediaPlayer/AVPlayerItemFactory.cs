using System.Collections.Generic;
using System.Threading.Tasks;
using AVFoundation;
using BMM.Api.Abstraction;
using BMM.Core.Extensions;
using BMM.UI.iOS.NewMediaPlayer.Interfaces;
using Foundation;

namespace BMM.UI.iOS.NewMediaPlayer
{
    public class AVPlayerItemFactory : IAVPlayerItemFactory
    {
        private readonly IMediaRequestHttpHeaders _headers;

        public AVPlayerItemFactory(IMediaRequestHttpHeaders headers)
        {
            _headers = headers;
        }

        public async Task<AVPlayerItem> Create(IMediaTrack mediaTrack)
        {
            var mediaUrl = MediaFileUrlHelper.GetUrlFor(mediaTrack);

            AVUrlAssetOptions options = null;
            
            if (!mediaTrack.IsDownloaded())
                options = GetOptionsWithHeaders(await _headers.GetHeaders());

            var asset = AVUrlAsset.Create(mediaUrl, options);
            return AVPlayerItem.FromAsset(asset);
        }

        public AVPlayerItem Create(string cacheFilePath)
        {
            var asset = AVUrlAsset.Create(new NSUrl(cacheFilePath, false));
            return AVPlayerItem.FromAsset(asset);
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