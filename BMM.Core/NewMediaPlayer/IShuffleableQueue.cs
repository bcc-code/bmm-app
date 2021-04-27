using BMM.Api.Abstraction;
using BMM.Core.NewMediaPlayer.Abstractions;

namespace BMM.Core.NewMediaPlayer
{
    // For Android the ExoPlayer takes care of the queue. But for iOS it needs to be implemented itself
    public interface IShuffleableQueue : IMediaQueue
    {
        void SetShuffle(bool isShuffleEnabled, IMediaTrack _currentTrack);

        void SetRepeatType(RepeatType type);

        bool IsShuffleEnabled { get; }

        RepeatType RepeatMode { get; }
    }
}