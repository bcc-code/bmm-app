using System.Collections.Generic;
using System.Threading.Tasks;
using AVFoundation;
using BMM.Api.Abstraction;

namespace BMM.UI.iOS.NewMediaPlayer.Interfaces
{
    public interface IAVPlayerItemRepository
    {
        Task AddAndLoad(IMediaTrack mediaTrack);
        Task<AVPlayerItem> Get(IMediaTrack mediaTrack);
        void SynchronizeCacheFiles();
        IList<string> GetAllCachedFiles(bool onlyFullyLoaded);
    }
}