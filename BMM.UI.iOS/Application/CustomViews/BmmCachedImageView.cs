using System;
using System.ComponentModel;
using CoreGraphics;
using FFImageLoading.Cross;
using Foundation;
using UIKit;

namespace BMM.UI.iOS
{
    [Register(nameof(BmmCachedImageView)), DesignTimeVisible(true)]
    public class BmmCachedImageView : MvxCachedImageView
    {
        public BmmCachedImageView()
        {
        }

        public BmmCachedImageView(IntPtr handle) : base(handle)
        {
        }

        public BmmCachedImageView(CGRect frame) : base(frame)
        {
        }

        public override UIImage? Image
        {
            get => base.Image;
            set
            {
                base.Image = value;
                ImageChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public event EventHandler ImageChanged;
    }
}