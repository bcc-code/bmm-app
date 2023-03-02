// WARNING
//
// This file has been generated automatically by Rider IDE
//   to store outlets and actions made in Xcode.
// If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace BMM.UI.iOS
{
	[Register ("YearInReviewCollectionViewCell")]
	partial class YearInReviewCollectionViewCell
	{
		[Outlet]
		UIKit.UIView ImageContainerView { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint ImageHeightConstraint { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint ImageWidthConstraint { get; set; }

		[Outlet]
		UIKit.UILabel SubtitleLabel { get; set; }

		[Outlet]
		FFImageLoading.Cross.MvxCachedImageView YearInReviewImageView { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (ImageContainerView != null) {
				ImageContainerView.Dispose ();
				ImageContainerView = null;
			}

			if (ImageHeightConstraint != null) {
				ImageHeightConstraint.Dispose ();
				ImageHeightConstraint = null;
			}

			if (ImageWidthConstraint != null) {
				ImageWidthConstraint.Dispose ();
				ImageWidthConstraint = null;
			}

			if (SubtitleLabel != null) {
				SubtitleLabel.Dispose ();
				SubtitleLabel = null;
			}

			if (YearInReviewImageView != null) {
				YearInReviewImageView.Dispose ();
				YearInReviewImageView = null;
			}
		}
	}
}
