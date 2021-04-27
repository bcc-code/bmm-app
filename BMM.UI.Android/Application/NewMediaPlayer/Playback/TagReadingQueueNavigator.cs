using Android.Support.V4.Media;
using Android.Support.V4.Media.Session;
using Com.Google.Android.Exoplayer2;
using Com.Google.Android.Exoplayer2.Ext.Mediasession;

namespace BMM.UI.Droid.Application.NewMediaPlayer.Playback
{
    /// <summary>
    /// Reads the MediaDescription from the tag. For this to work it is required that the PlaybackPreparer sets this tag correctly.
    /// </summary>
    public class TagReadingQueueNavigator : TimelineQueueNavigator
    {
        private readonly Timeline.Window _window;

        public TagReadingQueueNavigator(MediaSessionCompat mediaSession) : base(mediaSession)
        {
            _window = new Timeline.Window();
        }

        /// <summary>
        /// Change previousButton to act like PreviousOrSeekToStart.
        /// The default implementation has a bug so that it ignores if shuffle is enabled. This fixes it.
        /// It can be removed once we have updated ExoPlayer to include this fix: https://github.com/google/ExoPlayer/issues/5065
        /// (It's supposed to be included in ExoPlayer 2.9.1)
        /// </summary>
        public override long GetSupportedQueueNavigatorActions(IPlayer player)
        {
            long actions = 0;

            if (player == null)
            {
                return actions;
            }

            var isRepeatEnabled = player.RepeatMode != IPlayer.RepeatModeOff;
            if (player.IsCurrentWindowSeekable && !player.IsLiveStream() || player.PreviousWindowIndex > -1 || isRepeatEnabled)
            {
                actions = actions | PlaybackStateCompat.ActionSkipToPrevious;
            }

            if (player.CurrentTimeline.WindowCount <= 1)
            {
                return actions;
            }

            actions = actions | PlaybackStateCompat.ActionSkipToQueueItem;

            if (player.NextWindowIndex > -1 || isRepeatEnabled)
            {
                actions = actions | PlaybackStateCompat.ActionSkipToNext;
            }

            return actions;
        }

        public override MediaDescriptionCompat GetMediaDescription(IPlayer player, int windowIndex)
        {
            var item = player.CurrentTimeline.GetWindow(windowIndex, _window);
            var tag = item.Tag as MediaDescriptionCompat;
            return tag ?? HackForDescriptionForStreamingTrack;
        }

        /// <summary>
        /// For unknown reasons the HLS stream does not contain a tag the first time it is accessed. Therefore we use this hacky static variable as a work-around.
        /// </summary>
        public static MediaDescriptionCompat HackForDescriptionForStreamingTrack;
    }
}