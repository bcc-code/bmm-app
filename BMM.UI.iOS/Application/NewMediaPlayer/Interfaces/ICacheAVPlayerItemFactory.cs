using System.Threading.Tasks;
using BMM.Api.Abstraction;
using BMM.UI.iOS.NewMediaPlayer.CachePlayerItem;

namespace BMM.UI.iOS.NewMediaPlayer.Interfaces
{
    public interface ICacheAVPlayerItemFactory
    {
        Task<CacheAVPlayerItem> Create(IMediaTrack mediaTrack);
    }
}