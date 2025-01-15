// WARNING
//
// This file has been generated automatically by Rider IDE
//   to store outlets and actions made in Xcode.
// If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace BMM.UI.iOS.CustomViews.Swipes
{
	[Register ("SwipeMenuSimpleItem")]
	partial class SwipeMenuSimpleItem
	{
		[Outlet]
		UIKit.UIView BackgroundView { get; set; }

		[Outlet]
		UIKit.UILabel SwipeLabel { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (BackgroundView != null) {
				BackgroundView.Dispose ();
				BackgroundView = null;
			}

			if (SwipeLabel != null) {
				SwipeLabel.Dispose ();
				SwipeLabel = null;
			}

		}
	}
}
