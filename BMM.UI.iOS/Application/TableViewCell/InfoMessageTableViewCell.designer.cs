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
	[Register ("InfoMessageTableViewCell")]
	partial class InfoMessageTableViewCell
	{
		[Outlet]
		UIKit.UILabel InfoMessageLabel { get; set; }
		[Outlet]
		UIKit.UIButton InfoMessageButton { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (InfoMessageLabel != null) {
				InfoMessageLabel.Dispose ();
				InfoMessageLabel = null;
			}

			if (InfoMessageButton != null) {
				InfoMessageButton.Dispose();
				InfoMessageButton = null;
			}
		}
	}
}
