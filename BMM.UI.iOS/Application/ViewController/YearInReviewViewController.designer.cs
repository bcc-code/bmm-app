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
	[Register ("YearInReviewViewController")]
	partial class YearInReviewViewController
	{
		[Outlet]
		UIKit.UILabel DescriptionLabel { get; set; }

		[Outlet]
		UIKit.UIButton ShareButton { get; set; }

		[Outlet]
		UIKit.UICollectionView YearInReviewCollectionView { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint YearInReviewCollectionViewConstraint { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (DescriptionLabel != null) {
				DescriptionLabel.Dispose ();
				DescriptionLabel = null;
			}

			if (YearInReviewCollectionView != null) {
				YearInReviewCollectionView.Dispose ();
				YearInReviewCollectionView = null;
			}

			if (YearInReviewCollectionViewConstraint != null) {
				YearInReviewCollectionViewConstraint.Dispose ();
				YearInReviewCollectionViewConstraint = null;
			}

			if (ShareButton != null) {
				ShareButton.Dispose ();
				ShareButton = null;
			}

		}
	}
}
