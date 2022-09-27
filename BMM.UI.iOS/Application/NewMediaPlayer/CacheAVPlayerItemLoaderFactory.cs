using BMM.Api.Abstraction;
using BMM.UI.iOS.NewMediaPlayer.Interfaces;
using MvvmCross.Plugin.Messenger;

namespace BMM.UI.iOS.NewMediaPlayer
{
    public class CacheAVPlayerItemLoaderFactory : ICacheAVPlayerItemLoaderFactory
    {
        private readonly IMvxMessenger _mvxMessenger;
        private readonly IMediaRequestHttpHeaders _mediaRequestHttpHeaders;

        public CacheAVPlayerItemLoaderFactory(
            IMvxMessenger mvxMessenger, 
            IMediaRequestHttpHeaders mediaRequestHttpHeaders)
        {
            _mvxMessenger = mvxMessenger;
            _mediaRequestHttpHeaders = mediaRequestHttpHeaders;
        }
        
        public ICacheAVPlayerItemLoader Create(string uniqueKey)
        {
            return new CacheAVPlayerItemLoader(_mvxMessenger, _mediaRequestHttpHeaders, uniqueKey);
        }
    }
}