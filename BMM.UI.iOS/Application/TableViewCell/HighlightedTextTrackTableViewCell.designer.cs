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
	[Register ("HighlightedTextTrackTableViewCell")]
	partial class HighlightedTextTrackTableViewCell
	{
		[Outlet]
		UIKit.NSLayoutConstraint BottomSpacingConstraint { get; set; }

		[Outlet]
		BMM.UI.iOS.CustomViews.BmmFormattedLabel HighlightLabel { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint HighlightningsContainerHeightConstraint { get; set; }

		[Outlet]
		UIKit.UIScrollView LabelScrollView { get; set; }

		[Outlet]
		UIKit.UIView LeftGradientView { get; set; }

		[Outlet]
		UIKit.UIView RightGradientView { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (LabelScrollView != null) {
				LabelScrollView.Dispose ();
				LabelScrollView = null;
			}

			if (BottomSpacingConstraint != null) {
				BottomSpacingConstraint.Dispose ();
				BottomSpacingConstraint = null;
			}

			if (HighlightLabel != null) {
				HighlightLabel.Dispose ();
				HighlightLabel = null;
			}

			if (HighlightningsContainerHeightConstraint != null) {
				HighlightningsContainerHeightConstraint.Dispose ();
				HighlightningsContainerHeightConstraint = null;
			}

			if (LeftGradientView != null) {
				LeftGradientView.Dispose ();
				LeftGradientView = null;
			}

			if (RightGradientView != null) {
				RightGradientView.Dispose ();
				RightGradientView = null;
			}

		}
	}
}
