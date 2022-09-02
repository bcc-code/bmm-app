using AVFoundation;
using Foundation;

namespace BMM.UI.iOS.NewMediaPlayer.CachePlayerItem
{
    public class CacheAVPlayerItem : AVPlayerItem
    {
        private readonly CachePlayerItemResourceLoaderDelegate _cachePlayerItemResourceLoaderDelegate;

        public CacheAVPlayerItem(string url,
            CachePlayerItemResourceLoaderDelegate cachePlayerItemResourceLoaderDelegate,
            AVUrlAsset asset,
            params NSString[] automaticallyLoadedAssetKeys) : base(asset, automaticallyLoadedAssetKeys)
        {
            Url = url;
            _cachePlayerItemResourceLoaderDelegate = cachePlayerItemResourceLoaderDelegate;
            _cachePlayerItemResourceLoaderDelegate.Owner = this;
        }

        public string Url { get; }
        
        public bool IsDownloaded { get; set; }

        public void Download()
        {
            if (_cachePlayerItemResourceLoaderDelegate.Session == null)
                _cachePlayerItemResourceLoaderDelegate.StartDataRequest(Url);
        }
    }
}