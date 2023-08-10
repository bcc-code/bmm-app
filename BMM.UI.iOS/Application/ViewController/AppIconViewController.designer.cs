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
	[Register ("AppIconViewController")]
	partial class AppIconViewController
	{
		[Outlet]
		UIKit.UITableView AppIconTableView { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (AppIconTableView != null) {
                AppIconTableView.Dispose ();
                AppIconTableView = null;
			}

		}
	}
}
