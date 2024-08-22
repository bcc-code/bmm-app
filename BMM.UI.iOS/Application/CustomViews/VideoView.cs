using System;
using AVFoundation;
using CoreGraphics;
using CoreMedia;
using Foundation;
using UIKit;

namespace BMM.UI.iOS.CustomViews
{
    [Register(nameof(VideoView))]
    public class VideoView : UIView
    {
        private AVPlayerLayer _playerLayer;
        private bool _shouldPlayInLoop;
        private NSObject _didPlayToEndTimeToken;

        public VideoView(NSCoder coder) : base(coder)
        {
        }

        public VideoView()
        {
        }

        protected VideoView(NSObjectFlag t) : base(t)
        {
        }

        protected internal VideoView(ObjCRuntime.NativeHandle handle) : base(handle)
        {
        }

        public VideoView(CGRect frame) : base(frame)
        {
        }
        
        public void Configure(NSUrl url, bool shouldPlayInLoop)
        {
            _playerLayer?.RemoveFromSuperLayer();
            _playerLayer?.Player?.Dispose();
            _playerLayer?.Dispose();
            _didPlayToEndTimeToken?.Dispose();
            
            _shouldPlayInLoop = shouldPlayInLoop;
            
            _playerLayer = new AVPlayerLayer()
            {
                Player = AVPlayer.FromUrl(url)
            };
            
            _playerLayer.Frame = Bounds;
            _playerLayer.VideoGravity = AVLayerVideoGravity.Resize;
            Layer.InsertSublayer(_playerLayer, 0);
            _playerLayer.Player!.Play();

            _didPlayToEndTimeToken = AVPlayerItem.Notifications.ObserveDidPlayToEndTime(_playerLayer.Player.CurrentItem!, OnDidPlayToEndTime);
        }

        private void OnDidPlayToEndTime(object sender, NSNotificationEventArgs e)
        {
            var player = _playerLayer.Player;

            if (!_shouldPlayInLoop || player == null)
                return;
            
            player.Pause();
            player.Seek(CMTime.Zero);
            player.Play();
        }
    }
}