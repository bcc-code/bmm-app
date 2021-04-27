using System;
using System.Threading.Tasks;
using BMM.Core.NewMediaPlayer.Abstractions;
using UIKit;

namespace BMM.UI.iOS.NewMediaPlayer
{
    public class MediaRemoteControl : IMediaRemoteControl
    {
        private const long StepSeconds = 10;
        private readonly TimeSpan _seekInterval = TimeSpan.FromMilliseconds(1000);
        private readonly IMediaPlayer _mediaPlayer;

        // This variable makes it necessary for this class to be a singleton
        private bool _isSeeking;

        public MediaRemoteControl(IMediaPlayer mediaPlayer)
        {
            _mediaPlayer = mediaPlayer;
        }

        public void RemoteControlReceived(UIEvent uiEvent)
        {
            HandleRemoteControlEvent(uiEvent.Subtype);
        }

        private void HandleRemoteControlEvent(UIEventSubtype eventType)
        {
            switch (eventType)
            {
                case UIEventSubtype.RemoteControlBeginSeekingForward:
                    Task.Run(() => ExecuteWithIntervalWhileSeeking(() => _mediaPlayer.SkipForward(StepSeconds)));
                    break;

                case UIEventSubtype.RemoteControlBeginSeekingBackward:
                    Task.Run(() => ExecuteWithIntervalWhileSeeking(() => _mediaPlayer.SkipBackward(StepSeconds)));
                    break;

                case UIEventSubtype.RemoteControlEndSeekingForward:
                case UIEventSubtype.RemoteControlEndSeekingBackward:
                    EndSeeking();
                    break;
            }
        }

        private void EndSeeking()
        {
            _isSeeking = false;
        }

        private async Task ExecuteWithIntervalWhileSeeking(Action taskToExecute)
        {
            if (_isSeeking)
            {
                return;
            }

            _isSeeking = true;

            while (_isSeeking)
            {
                taskToExecute();
                await Task.Delay(_seekInterval);
            }
        }
    }
}