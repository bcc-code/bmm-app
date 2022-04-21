using System;
using BMM.Core.Extensions;
using BMM.Core.Messages.MediaPlayer;
using BMM.Core.NewMediaPlayer;
using BMM.Core.NewMediaPlayer.Abstractions;
using PS = Android.Support.V4.Media.Session.PlaybackStateCompat;

namespace BMM.UI.Droid.Application.NewMediaPlayer.Notification
{
    public static class PlaybackStateCompatExtensions
    {
        public static bool IsPrepared(this PS state)
        {
            return state.State == PS.StateBuffering || state.State == PS.StatePlaying || state.State == PS.StatePaused;
        }

        public static bool IsPlaying(this PS state)
        {
            return state.State == PS.StateBuffering || state.State == PS.StatePlaying;
        }

        public static PlayStatus PlayStatus(this PS state)
        {
            if (state.State == PS.StateBuffering)
                return Core.NewMediaPlayer.PlayStatus.Buffering;
            if (state.State == PS.StatePlaying)
                return Core.NewMediaPlayer.PlayStatus.Playing;
            if (state.State == PS.StatePaused)
                return Core.NewMediaPlayer.PlayStatus.Paused;
            if (state.State == PS.StateConnecting)
                return Core.NewMediaPlayer.PlayStatus.Loading;
            if (state.State == PS.StateError)
                return Core.NewMediaPlayer.PlayStatus.Failed;
            return Core.NewMediaPlayer.PlayStatus.Unknown;
        }

        public static bool IsPlayEnabled(this PS state)
        {
            return state.Allows(PS.ActionPlay) || state.Allows(PS.ActionPlayPause) && state.State == PS.StatePaused;
        }

        public static bool IsPauseEnabled(this PS state)
        {
            return state.Allows(PS.ActionPause) || (state.Allows(PS.ActionPlayPause) && state.State == PS.StateBuffering || state.State == PS.StatePlaying);
        }

        public static bool IsSkipToNextEnabled(this PS state)
        {
            return state.Allows(PS.ActionSkipToNext);
        }

        public static bool IsSkipToPreviousEnabled(this PS state)
        {
            return state.Allows(PS.ActionSkipToPrevious);
        }

        public static bool Allows(this PS state, long playbackState)
        {
            return (state.Actions & playbackState) != 0L;
        }

        public static IPlaybackState ToPlaybackState(this PS state, IMediaQueue mediaQueue, decimal desiredPlaybackSpeed)
        {
            return new PlaybackState
            {
                IsPlaying = state.IsPlaying(),
                IsSkipToNextEnabled = state.IsSkipToNextEnabled(),
                IsSkipToPreviousEnabled = state.IsSkipToPreviousEnabled(),
                CurrentIndex = state.ActiveQueueItemId,
                QueueLength = mediaQueue.Tracks.Count,
                CurrentPosition = state.Position,
                BufferedPosition = state.BufferedPosition.WithPossibleLowestValue(0),
                PlayStatus = state.PlayStatus(),
                DesiredPlaybackRate = desiredPlaybackSpeed
            };
        }
    }
}