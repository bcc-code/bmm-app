// WARNING
//
// This file has been generated automatically by Rider IDE
//   to store outlets and actions made in Xcode.
// If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
namespace BMM.UI.iOS.CustomViews.Swipes
{
	[Register ("SwipeMenuItem")]
	partial class SwipeMenuItem
	{
		[Outlet]
		UIKit.UIView BackgroundView { get; set; }

		[Outlet]
		UIKit.UIView ContentContainer { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint SeparatorHeightConstraint { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint SeparatorLeadingConstraint { get; set; }

		[Outlet]
		UIKit.UIView SeparatorView { get; set; }

		[Outlet]
		UIKit.UIImageView SwipeIcon { get; set; }

		[Outlet]
		UILabel SwipeLabel { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (BackgroundView != null) {
				BackgroundView.Dispose ();
				BackgroundView = null;
			}

			if (SeparatorLeadingConstraint != null) {
				SeparatorLeadingConstraint.Dispose ();
				SeparatorLeadingConstraint = null;
			}

			if (SeparatorView != null) {
				SeparatorView.Dispose ();
				SeparatorView = null;
			}

			if (SwipeIcon != null) {
				SwipeIcon.Dispose ();
				SwipeIcon = null;
			}

			if (SeparatorHeightConstraint != null) {
				SeparatorHeightConstraint.Dispose ();
				SeparatorHeightConstraint = null;
			}

			if (SwipeLabel != null) {
				SwipeLabel.Dispose ();
				SwipeLabel = null;
			}

			if (ContentContainer != null) {
				ContentContainer.Dispose ();
				ContentContainer = null;
			}

		}
	}
}
