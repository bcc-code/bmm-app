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
	[Register ("AslaksenTableViewCell")]
	partial class AslaksenTableViewCell
	{
		[Outlet]
		UIKit.UIButton AslaksenPlayNewestButton { get; set; }

		[Outlet]
		UIKit.UIButton AslaksenPlayRandomButton { get; set; }

		[Outlet]
		UIKit.UIButton AslaksenShowAllButton { get; set; }

		[Outlet]
		UIKit.UILabel AslaksenTitle { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (AslaksenShowAllButton != null) {
				AslaksenShowAllButton.Dispose ();
				AslaksenShowAllButton = null;
			}

			if (AslaksenTitle != null) {
				AslaksenTitle.Dispose ();
				AslaksenTitle = null;
			}

			if (AslaksenPlayRandomButton != null) {
				AslaksenPlayRandomButton.Dispose ();
				AslaksenPlayRandomButton = null;
			}

			if (AslaksenPlayNewestButton != null) {
				AslaksenPlayNewestButton.Dispose ();
				AslaksenPlayNewestButton = null;
			}

		}
	}
}
