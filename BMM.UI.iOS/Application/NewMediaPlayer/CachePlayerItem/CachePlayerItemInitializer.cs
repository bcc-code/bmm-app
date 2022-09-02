using AVFoundation;
using BMM.UI.iOS.Extensions;
using CoreFoundation;
using Foundation;

namespace BMM.UI.iOS.NewMediaPlayer.CachePlayerItem
{
    public static class CachePlayerItemInitializer
    {
        private const string CachingPlayerItemScheme = "cachingPlayerItemScheme";

        public static CacheAVPlayerItem Init(NSUrl url, NSDictionary headers)
        {
            var urlWithCustomScheme = url.WithScheme(CachingPlayerItemScheme);

            var asset = AVUrlAsset.Create(urlWithCustomScheme);
            var resourceLoaderDelegate = new CachePlayerItemResourceLoaderDelegate(headers);

            asset.ResourceLoader.SetDelegate(resourceLoaderDelegate, DispatchQueue.MainQueue);

            return new CacheAVPlayerItem(url.ToString(),
                resourceLoaderDelegate,
                asset,
                null);
        }
    }
}
