using System;
using Android.Media;

namespace BMM.UI.Droid.Application.Listeners
{
    public class OnPreparedListeners : Java.Lang.Object, MediaPlayer.IOnPreparedListener
    {
        private readonly Action<MediaPlayer> _onPreparedAction;

        public OnPreparedListeners(Action<MediaPlayer> onPreparedAction)
        {
            _onPreparedAction = onPreparedAction;
        }
        
        public void OnPrepared(MediaPlayer mp)
        {
            _onPreparedAction?.Invoke(mp);
        }
    }
}