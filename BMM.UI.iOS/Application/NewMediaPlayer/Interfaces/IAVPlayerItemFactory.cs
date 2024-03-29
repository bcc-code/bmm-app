using System.Threading.Tasks;
using AVFoundation;
using BMM.Api.Abstraction;

namespace BMM.UI.iOS.NewMediaPlayer.Interfaces
{
    public interface IAVPlayerItemFactory
    {
        Task<AVPlayerItem> Create(IMediaTrack mediaTrack);
        AVPlayerItem Create(string cacheFilePath);
    }
}