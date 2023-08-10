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
	[Register ("AchievementDetailsViewController")]
	partial class AchievementDetailsViewController
	{
		[Outlet]
		UIKit.UIButton ActivateButton { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint CloseIconHeightConstraint { get; set; }

		[Outlet]
		UIKit.UIView CloseIconView { get; set; }

		[Outlet]
		UIKit.UILabel DescriptionLabel { get; set; }

		[Outlet]
		FFImageLoading.Cross.MvxCachedImageView IconImage { get; set; }

		[Outlet]
		UIKit.UILabel NameLabel { get; set; }

		[Outlet]
		UIKit.UILabel StatusLabel { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (ActivateButton != null) {
				ActivateButton.Dispose ();
				ActivateButton = null;
			}

			if (CloseIconView != null) {
				CloseIconView.Dispose ();
				CloseIconView = null;
			}

			if (DescriptionLabel != null) {
				DescriptionLabel.Dispose ();
				DescriptionLabel = null;
			}

			if (IconImage != null) {
				IconImage.Dispose ();
				IconImage = null;
			}

			if (NameLabel != null) {
				NameLabel.Dispose ();
				NameLabel = null;
			}

			if (StatusLabel != null) {
				StatusLabel.Dispose ();
				StatusLabel = null;
			}

			if (CloseIconHeightConstraint != null) {
				CloseIconHeightConstraint.Dispose ();
				CloseIconHeightConstraint = null;
			}

		}
	}
}
