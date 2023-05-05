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
	[Register ("HighlightedTextTableViewCell")]
	partial class HighlightedTextTableViewCell
	{
		[Outlet]
		BMM.UI.iOS.CustomViews.BmmFormattedLabel HighlightedTextLabel { get; set; }

		[Outlet]
		UIKit.UILabel PositionLabel { get; set; }

		[Outlet]
		UIKit.UIButton ShareButton { get; set; }

		[Outlet]
		UIKit.UIView ShareButtonArea { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (HighlightedTextLabel != null) {
				HighlightedTextLabel.Dispose ();
				HighlightedTextLabel = null;
			}

			if (PositionLabel != null) {
				PositionLabel.Dispose ();
				PositionLabel = null;
			}

			if (ShareButton != null) {
				ShareButton.Dispose ();
				ShareButton = null;
			}

			if (ShareButtonArea != null) {
				ShareButtonArea.Dispose ();
				ShareButtonArea = null;
			}

		}
	}
}
