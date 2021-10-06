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
	[Register ("DiscoverSectionHeaderTableViewCell")]
	partial class DiscoverSectionHeaderTableViewCell
	{
		[Outlet]
		BMM.UI.iOS.Separator Divider { get; set; }

		[Outlet]
		UIKit.UIButton LinkButton { get; set; }

		[Outlet]
		UIKit.UILabel Titel { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (LinkButton != null) {
				LinkButton.Dispose ();
				LinkButton = null;
			}

			if (Titel != null) {
				Titel.Dispose ();
				Titel = null;
			}

			if (Divider != null) {
				Divider.Dispose ();
				Divider = null;
			}

		}
	}
}
