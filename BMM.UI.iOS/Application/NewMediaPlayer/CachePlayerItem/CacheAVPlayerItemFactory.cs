using System.Collections.Generic;
using System.Threading.Tasks;
using BMM.Api.Abstraction;
using BMM.Core.Extensions;
using BMM.UI.iOS.NewMediaPlayer.CachePlayerItem;
using BMM.UI.iOS.NewMediaPlayer.Interfaces;
using Foundation;

namespace BMM.UI.iOS.NewMediaPlayer
{
    public class CacheAVPlayerItemFactory : ICacheAVPlayerItemFactory
    {
        private readonly IMediaRequestHttpHeaders _headers;

        public CacheAVPlayerItemFactory(IMediaRequestHttpHeaders headers)
        {
            _headers = headers;
        }

        public async Task<CacheAVPlayerItem> Create(IMediaTrack mediaTrack)
        {
            var mediaUrl = MediaFileUrlHelper.GetUrlFor(mediaTrack);

            NSMutableDictionary options = null;

            if (!mediaTrack.IsDownloaded())
                options = GetOptionsWithHeaders(await _headers.GetHeaders());

            return CachePlayerItemInitializer.Init(mediaUrl, options);
        }

        private NSMutableDictionary GetOptionsWithHeaders(IEnumerable<KeyValuePair<string, string>> headers)
        {
            var headerDictionary = new NSMutableDictionary();
            foreach (var header in headers)
            {
                headerDictionary.Add(new NSString(header.Key), new NSString(header.Value));
            }

            return headerDictionary;
        }
    }
}