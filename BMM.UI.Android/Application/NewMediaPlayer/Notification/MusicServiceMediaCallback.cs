using System;
using Android.Support.V4.Media;
using Android.Support.V4.Media.Session;

namespace BMM.UI.Droid.Application.NewMediaPlayer.Notification
{
    public class MusicServiceMediaCallback : MediaControllerCompat.Callback
    {
        private int? _previousState;

        public Action<PlaybackStateCompat> OnPlaybackStateChangedImpl { private get; set; }
        public Action<MediaMetadataCompat> OnMetadataChangedImpl { private get; set; }
        public Action OnSessionDestroyedImpl { get; set; }

        public override void OnPlaybackStateChanged(PlaybackStateCompat state)
        {
            if (_previousState == state.State)
                return;

            _previousState = state.State;
            OnPlaybackStateChangedImpl?.Invoke(state);
        }

        public override void OnMetadataChanged(MediaMetadataCompat meta)
        {
            OnMetadataChangedImpl?.Invoke(meta);
        }

        public override void OnSessionDestroyed()
        {
            OnSessionDestroyedImpl?.Invoke();
        }
    }
}