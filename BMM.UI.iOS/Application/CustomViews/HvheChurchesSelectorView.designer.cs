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
	[Register ("HvheChurchesSelectorView")]
	partial class HvheChurchesSelectorView
	{
		[Outlet]
		UIKit.UIView LeftIndicatorView { get; set; }

		[Outlet]
		UIKit.UIView LeftItemContainer { get; set; }

		[Outlet]
		UIKit.UILabel LeftItemLabel { get; set; }

		[Outlet]
		UIKit.UIView RightIndicatorView { get; set; }

		[Outlet]
		UIKit.UIView RightItemContainer { get; set; }

		[Outlet]
		UIKit.UILabel RightItemLabel { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (LeftIndicatorView != null) {
				LeftIndicatorView.Dispose ();
				LeftIndicatorView = null;
			}

			if (LeftItemContainer != null) {
				LeftItemContainer.Dispose ();
				LeftItemContainer = null;
			}

			if (LeftItemLabel != null) {
				LeftItemLabel.Dispose ();
				LeftItemLabel = null;
			}

			if (RightIndicatorView != null) {
				RightIndicatorView.Dispose ();
				RightIndicatorView = null;
			}

			if (RightItemContainer != null) {
				RightItemContainer.Dispose ();
				RightItemContainer = null;
			}

			if (RightItemLabel != null) {
				RightItemLabel.Dispose ();
				RightItemLabel = null;
			}

		}
	}
}
