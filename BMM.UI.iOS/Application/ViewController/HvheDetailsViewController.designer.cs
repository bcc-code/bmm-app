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
	[Register ("HvheDetailsViewController")]
	partial class HvheDetailsViewController
	{
		[Outlet]
		UIKit.UIView CloseIconView { get; set; }

		[Outlet]
		UIKit.UITableView ContentTableView { get; set; }

		[Outlet]
		UIKit.UILabel PageTitleLabel { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (CloseIconView != null) {
				CloseIconView.Dispose ();
				CloseIconView = null;
			}

			if (ContentTableView != null) {
				ContentTableView.Dispose ();
				ContentTableView = null;
			}

			if (PageTitleLabel != null) {
				PageTitleLabel.Dispose ();
				PageTitleLabel = null;
			}

		}
	}
}
