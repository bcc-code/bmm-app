using MvvmCross.Platforms.Ios.Binding.Views;
using CoreAnimation;
using Foundation;
using System;
using UIKit;

namespace BMM.UI.iOS
{
    public partial class LoadingTableViewCell : MvxTableViewCell
    {
        public static readonly UINib Nib = UINib.FromName("LoadingTableViewCell", NSBundle.MainBundle);
        public static readonly NSString Key = new NSString("LoadingTableViewCell");

        public LoadingTableViewCell(IntPtr handle) : base(handle)
        {
        }

        public static LoadingTableViewCell Create()
        {
            return (LoadingTableViewCell)Nib.Instantiate(null, null)[0];
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