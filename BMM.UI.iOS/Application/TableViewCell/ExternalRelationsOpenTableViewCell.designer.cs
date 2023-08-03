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
	[Register ("ExternalRelationsOpenTableViewCell")]
	partial class ExternalRelationsOpenTableViewCell
	{
		[Outlet]
		UIKit.UIView OpenButton { get; set; }

		[Outlet]
		UIKit.UILabel OpenLabel { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (OpenButton != null) {
				OpenButton.Dispose ();
				OpenButton = null;
			}

			if (OpenLabel != null) {
				OpenLabel.Dispose ();
				OpenLabel = null;
			}

		}
	}
}
