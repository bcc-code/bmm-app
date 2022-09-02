using System.Collections.Generic;
using System.Threading.Tasks;
using AVFoundation;
using BMM.Api.Abstraction;
using BMM.Core.Extensions;
using BMM.UI.iOS.NewMediaPlayer.CachePlayerItem;
using BMM.UI.iOS.NewMediaPlayer.Interfaces;

namespace BMM.UI.iOS.NewMediaPlayer
{
    public class AVPlayerItemRepository : IAVPlayerItemRepository
    {
        private readonly ICacheAVPlayerItemFactory _cacheAVPlayerItemFactory;
        private readonly IAVPlayerItemFactory _avPlayerItemFactory;
        private readonly Dictionary<string, CacheAVPlayerItem> _cacheAVPlayerItems = new Dictionary<string, CacheAVPlayerItem>();

        public AVPlayerItemRepository(
            ICacheAVPlayerItemFactory cacheAVPlayerItemFactory,
            IAVPlayerItemFactory avPlayerItemFactory)
        {
            _cacheAVPlayerItemFactory = cacheAVPlayerItemFactory;
            _avPlayerItemFactory = avPlayerItemFactory;
        }
        
        public async Task AddAndLoad(IMediaTrack mediaTrack)
        {
            if (_cacheAVPlayerItems.ContainsKey(mediaTrack.GetUniqueKey) || mediaTrack.IsDownloaded())
                return;

            var cacheAVPlayerItem = await _cacheAVPlayerItemFactory.Create(mediaTrack);
            cacheAVPlayerItem.Download();
            _cacheAVPlayerItems.TryAdd(mediaTrack.GetUniqueKey, cacheAVPlayerItem);
        }

        public async Task<AVPlayerItem> Get(IMediaTrack mediaTrack)
        {
            AVPlayerItem avPlayerItem = null;
            
            if (mediaTrack.IsDownloaded())
                avPlayerItem = await _avPlayerItemFactory.Create(mediaTrack);
            
            if (_cacheAVPlayerItems.TryGetValue(mediaTrack.GetUniqueKey, out var cacheAVPlayerItemToReturn) && cacheAVPlayerItemToReturn.IsDownloaded)
                avPlayerItem = cacheAVPlayerItemToReturn;

            return avPlayerItem ?? await _avPlayerItemFactory.Create(mediaTrack);
        }
    }
}