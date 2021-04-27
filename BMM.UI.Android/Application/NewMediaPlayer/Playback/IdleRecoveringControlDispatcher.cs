using System;
using BMM.UI.Droid.Application.NewMediaPlayer.Service;
using Com.Google.Android.Exoplayer2;

namespace BMM.UI.Droid.Application.NewMediaPlayer.Playback
{
    /// <summary>
    /// <inheritdoc cref="IdleRecoveringQueueNavigator"/>
    /// </summary>
    public class IdleRecoveringControlDispatcher : DefaultControlDispatcher
    {
        private readonly MediaSourceSetter _mediaSourceSetter;

        public IdleRecoveringControlDispatcher(MediaSourceSetter mediaSourceSetter)
        {
            _mediaSourceSetter = mediaSourceSetter;
        }

        public override bool DispatchSetPlayWhenReady(IPlayer player, bool playWhenReady)
        {
            RecoverFromIdleIfNecessary(() => player.CurrentWindowIndex,
                player,
                () => base.DispatchSetPlayWhenReady(player, playWhenReady),
                player.ContentPosition);
            return true;
        }

        public override bool DispatchSeekTo(IPlayer player, int windowIndex, long positionMs)
        {
            RecoverFromIdleIfNecessary(() => player.CurrentWindowIndex,
                player,
                () => base.DispatchSeekTo(player, windowIndex, positionMs),
                player.ContentPosition);
            return true;
        }

        private void RecoverFromIdleIfNecessary(Func<int> newIndex, IPlayer player, Action nonErrorAction, long seekToPosition = 0)
        {
            IdleRecoveringQueueNavigator.RecoverFromIdleIfNecessary(_mediaSourceSetter, newIndex, player, nonErrorAction, seekToPosition);
        }
    }
}