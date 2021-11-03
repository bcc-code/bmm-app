using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BMM.Api.Abstraction;
using BMM.Core.Messages.MediaPlayer;

namespace BMM.Core.NewMediaPlayer.Abstractions
{
    public interface IMediaPlayer : ICommonMediaPlayer
    {
        void ViewHasBeenDestroyed();

        void Play();

        Task Play(IList<IMediaTrack> mediaTracks, IMediaTrack currentTrack, string playbackOrigin, long startTimeInMs = 0);

        void Pause();

        void ToggleRepeatType();

        void ToggleShuffle();

        void JumpForward();

        void JumpBackward();

        void SkipForward(double timeInSeconds);

        void SkipBackward(double timeInSeconds);
    }

    public interface IPlatformSpecificMediaPlayer : ICommonMediaPlayer
    {
        void SetShuffle(bool isShuffleEnabled);

        void SetRepeatType(RepeatType type);

        Action ContinuingPreviousSession { get; set; }
    }

    /// <summary>
    /// The part of the interface that is shared between <see cref="IMediaPlayer"/> and <see cref="IPlatformSpecificMediaPlayer"/>.
    /// </summary>
    public interface ICommonMediaPlayer
    {
        bool IsPlaying { get; }

        RepeatType RepeatType { get; }

        bool IsShuffleEnabled { get; }

        IPlaybackState PlaybackState { get; }

        ITrackModel CurrentTrack { get; }

        Task Play(IList<IMediaTrack> mediaTracks, IMediaTrack currentTrack, long startTimeInMs = 0);

        Task PrepareToPlay(IList<IMediaTrack> mediaTracks, IMediaTrack currentTrack, string playbackOrigin, long startTimeInMs = 0);

        void PlayPause();

        /// <summary>
        /// Use it in combination with logout since it does not hide the miniplayer
        /// </summary>
        void Stop();

        void PlayNext();

        void PlayPrevious();

        void PlayPreviousOrSeekToStart();

        Task<bool> AddToEndOfQueue(IMediaTrack track, string playbackOrigin);

        Task<bool> QueueToPlayNext(IMediaTrack track, string playbackOrigin);

        void SeekTo(long newPosition);

        Task ShuffleList(IList<IMediaTrack> tracks, string playbackOrigin);
    }
}
