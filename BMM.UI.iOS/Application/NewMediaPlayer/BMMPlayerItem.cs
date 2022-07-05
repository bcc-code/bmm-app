using System;
using AVFoundation;
using BMM.Api.Abstraction;
using Foundation;

namespace BMM.UI.iOS.NewMediaPlayer
{
    public class BMMPlayerItem : AVPlayerItem
    {
        protected BMMPlayerItem(NSObjectFlag t) : base(t)
        {
        }

        protected internal BMMPlayerItem(IntPtr handle) : base(handle)
        {
        }

        public BMMPlayerItem(NSUrl URL) : base(URL)
        {
        }

        public BMMPlayerItem(AVAsset asset, IMediaTrack mediaTrack) : base(asset)
        {
            MediaTrack = mediaTrack;
        }

        public BMMPlayerItem(AVAsset asset, params NSString[]? automaticallyLoadedAssetKeys) : base(asset, automaticallyLoadedAssetKeys)
        {
        }
        
        public IMediaTrack MediaTrack { get; }
        public bool AreObserversAttached { get; set; }
    }
}