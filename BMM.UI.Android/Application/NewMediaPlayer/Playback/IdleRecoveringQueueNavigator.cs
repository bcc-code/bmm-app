using System;
using Android.Support.V4.Media.Session;
using BMM.UI.Droid.Application.NewMediaPlayer.Service;
using Com.Google.Android.Exoplayer2;

namespace BMM.UI.Droid.Application.NewMediaPlayer.Playback
{
    /// <summary>
    /// After an error occurs the ExoPlayer is left in an invalid state. This decorator makes sure that it recovers after an error has happened.
    /// The idea came from this post: https://github.com/google/ExoPlayer/issues/4343
    /// ToDo: Check if it's still needed or if the ExoPlayer update fixed this problem in the meantime.
    /// </summary>
    public class IdleRecoveringQueueNavigator : TagReadingQueueNavigator
    {
        private readonly MediaSourceSetter _mediaSourceSetter;

        public IdleRecoveringQueueNavigator(MediaSessionCompat mediaSession, MediaSourceSetter mediaSourceSetter) : base(mediaSession)
        {
            _mediaSourceSetter = mediaSourceSetter;
        }

        public override void OnSkipToNext(IPlayer player, IControlDispatcher controlDispatcher)
        {
            RecoverFromIdleIfNecessary(() => player.NextWindowIndex, player, () => base.OnSkipToNext(player,controlDispatcher));
        }

        public override void OnSkipToPrevious(IPlayer player, IControlDispatcher controlDispatcher)
        {
            RecoverFromIdleIfNecessary(() => player.PreviousWindowIndex, player, () =>
            {
                if (player.IsLiveStream())
                    // Never seek to start when it's a livestream
                    player.SeekTo(player.PreviousWindowIndex, 0);
                else
                    base.OnSkipToPrevious(player, controlDispatcher);
            });
        }

        public override void OnSkipToQueueItem(IPlayer player, IControlDispatcher controlDispatcher, long id)
        {
            RecoverFromIdleIfNecessary(() => (int)id, player, () => base.OnSkipToQueueItem(player, controlDispatcher, id));
        }

        private void RecoverFromIdleIfNecessary(Func<int> newIndex, IPlayer player, Action nonErrorAction)
        {
            RecoverFromIdleIfNecessary(_mediaSourceSetter, newIndex, player, nonErrorAction);
        }

        public static void RecoverFromIdleIfNecessary(MediaSourceSetter mediaSourceSetter, Func<int> newIndex, IPlayer player, Action nonErrorAction, long seekToPosition = 0)
        {
            // if an error occurs the player goes into idle state, so we don't need to handle it specifically
            if (player is IExoPlayer exoPlayer && player.PlaybackState == IPlayer.StateIdle)
            {
                var nextIndex = newIndex();
                exoPlayer.Prepare(mediaSourceSetter.Get);
                exoPlayer.SeekTo(nextIndex, seekToPosition);
                exoPlayer.PlayWhenReady = true;
            }
            else
            {
                nonErrorAction();
            }
        }
    }
}