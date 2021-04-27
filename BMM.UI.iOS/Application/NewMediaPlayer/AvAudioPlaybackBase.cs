using System;
using AVFoundation;
using CoreFoundation;
using CoreMedia;
using Foundation;

namespace BMM.UI.iOS.NewMediaPlayer
{
    public abstract class AvAudioPlaybackBase : NSObject
    {
        private AVPlayer _player;

        private NSObject _periodicTimeObserverObject;

        public NSKeyValueObservingOptions InitialAndNewObservingOptions = NSKeyValueObservingOptions.New | NSKeyValueObservingOptions.Initial;

        public static readonly NSString StatusObservationContext = new NSString("Status");
        public const string StatusObserver = "status";

        public static readonly NSString RateObservationContext = new NSString("Rate");
        public const string RateObserver = "rate";

        public static readonly NSString LoadedTimeRangesObservationContext = new NSString("TimeRanges");
        public const string LoadedTimeRangesObserver = "loadedTimeRanges";

        protected AVPlayer Player
        {
            get
            {
                if (_player == null)
                    InitializePlayer();

                return _player;
            }
        }

        protected void InitializePlayer()
        {
            DeinitializePlayer();

            _player = new AVPlayer
            {
                // This setting is for video playback and by default it's set to true. Since we only play audio we set it to false.
                // If you would want to use it you also need the "external-accessory" permission in UIBackgroundModes in Info.plist.
                // Without that permission audio playback over AirPlay fails because of the missing permission.
                AllowsExternalPlayback = false
            };

            if (VersionHelper.SupportsAutomaticWaitPlayerProperty)
            {
                _player.AutomaticallyWaitsToMinimizeStalling = false;
            }

            var avSession = AVAudioSession.SharedInstance();

            NSError activationError;
            // By setting the Audio Session category to AVAudioSessionCategoryPlayback, audio will continue to play when the silent switch is enabled, or when the screen is locked.
            if (VersionHelper.SupportsAirPlayTwo)
                avSession.SetCategory(AVAudioSession.CategoryPlayback, AVAudioSession.ModeDefault, AVAudioSessionRouteSharingPolicy.LongForm, 0, out activationError);
            else
                avSession.SetCategory(AVAudioSessionCategory.Playback);

            avSession.SetActive(true, out activationError);
            if (activationError != null)
            {
                Console.WriteLine("Could not activate audio session {0}", activationError.LocalizedDescription);
            }

            Player.AddObserver(this, RateObserver, InitialAndNewObservingOptions, RateObservationContext.Handle);

            _periodicTimeObserverObject = Player.AddPeriodicTimeObserver(new CMTime(1, 4), DispatchQueue.MainQueue, ObservePeriodicTimeEvent);
        }

        protected void DeinitializePlayer()
        {
            if (_player is null)
                return;

            _player.RemoveTimeObserver(_periodicTimeObserverObject);
            _player.RemoveObserver(this, RateObserver, RateObservationContext.Handle);

            _player.Dispose();
            _player = null;
        }

        public override void ObserveValue(NSString keyPath, NSObject ofObject, NSDictionary change, IntPtr context)
        {
            switch (keyPath)
            {
                case StatusObserver:
                    ObserveStatus();
                    return;

                case LoadedTimeRangesObserver:
                    ObserveLoadedTimeRanges();
                    return;

                case RateObserver:
                    ObserveRate();
                    return;

                default:
                    Console.WriteLine("Observer triggered for {0} not resolved ...", keyPath);
                    return;
            }
        }

        public abstract void ObserveStatus();

        public abstract void ObserveLoadedTimeRanges();

        public abstract void ObserveRate();

        public abstract void ObservePeriodicTimeEvent(CMTime obj);
    }
}