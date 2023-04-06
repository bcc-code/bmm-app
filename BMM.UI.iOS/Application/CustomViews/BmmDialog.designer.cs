// WARNING
//
// This file has been generated automatically by Rider IDE
//   to store outlets and actions made in Xcode.
// If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace BMM.UI.iOS.CustomViews
{
	[Register ("BmmDialog")]
	partial class BmmDialog
	{
		[Outlet]
		UIKit.UIButton CloseButton { get; set; }

		[Outlet]
		UIKit.UIView ContainerView { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint ContainerViewBottomConstraint { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint ContainerViewLeadingConstraint { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint ContainerViewTopConstraint { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint ContainerViewTrailingConstraint { get; set; }

		[Outlet]
		UIKit.UILabel HeaderLabel { get; set; }

		[Outlet]
		UIKit.UIView PopupView { get; set; }

		[Outlet]
		UIKit.UILabel SubtitleLabel { get; set; }

		[Outlet]
		UIKit.UILabel TitleLabel { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (CloseButton != null) {
				CloseButton.Dispose ();
				CloseButton = null;
			}

			if (ContainerView != null) {
				ContainerView.Dispose ();
				ContainerView = null;
			}

			if (ContainerViewBottomConstraint != null) {
				ContainerViewBottomConstraint.Dispose ();
				ContainerViewBottomConstraint = null;
			}

			if (ContainerViewLeadingConstraint != null) {
				ContainerViewLeadingConstraint.Dispose ();
				ContainerViewLeadingConstraint = null;
			}

			if (ContainerViewTopConstraint != null) {
				ContainerViewTopConstraint.Dispose ();
				ContainerViewTopConstraint = null;
			}

			if (ContainerViewTrailingConstraint != null) {
				ContainerViewTrailingConstraint.Dispose ();
				ContainerViewTrailingConstraint = null;
			}

			if (HeaderLabel != null) {
				HeaderLabel.Dispose ();
				HeaderLabel = null;
			}

			if (PopupView != null) {
				PopupView.Dispose ();
				PopupView = null;
			}

			if (TitleLabel != null) {
				TitleLabel.Dispose ();
				TitleLabel = null;
			}

			if (SubtitleLabel != null) {
				SubtitleLabel.Dispose ();
				SubtitleLabel = null;
			}

		}
	}
}
