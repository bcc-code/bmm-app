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
	[Register ("HvheHeaderTableViewCell")]
	partial class HvheHeaderTableViewCell
	{
		[Outlet]
		UIKit.UILabel LeftItemLabel { get; set; }

		[Outlet]
		UIKit.UILabel RightItemLabel { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (LeftItemLabel != null) {
				LeftItemLabel.Dispose ();
				LeftItemLabel = null;
			}

			if (RightItemLabel != null) {
				RightItemLabel.Dispose ();
				RightItemLabel = null;
			}

		}
	}
}
