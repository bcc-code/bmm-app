using CoreAnimation;
using Foundation;
using System;

namespace BMM.UI.iOS
{
    public partial class LoadingTableViewCell : BaseBMMTableViewCell
    {
        public static readonly NSString Key = new NSString(nameof(LoadingTableViewCell));

        public LoadingTableViewCell(IntPtr handle) : base(handle)
        {
        }

        public void AnimateSpinner()
        {
            CABasicAnimation rotationAnimation = CABasicAnimation.FromKeyPath("transform.rotation");
            rotationAnimation.To = NSNumber.FromDouble(Math.PI * 2); // full rotation (in radians)
            rotationAnimation.RepeatCount = int.MaxValue; // repeat forever
            rotationAnimation.Duration = 1;
            // Give the added animation a key for referencing it later (to remove, in this case).
            loadingSpinnerImageView.Layer.AddAnimation(rotationAnimation, "rotationAnimation");
        }
    }
}