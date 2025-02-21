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
	[Register ("HvheBoysVsGirlsTableViewCell")]
	partial class HvheBoysVsGirlsTableViewCell
	{
		[Outlet]
		UIKit.UILabel BoysLabel { get; set; }

		[Outlet]
		UIKit.UIView BoysPointsContainer { get; set; }

		[Outlet]
		UIKit.UILabel GirlsLabel { get; set; }

		[Outlet]
		UIKit.UIView GirlsPointsContainer { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (BoysLabel != null) {
				BoysLabel.Dispose ();
				BoysLabel = null;
			}

			if (GirlsLabel != null) {
				GirlsLabel.Dispose ();
				GirlsLabel = null;
			}

			if (BoysPointsContainer != null) {
				BoysPointsContainer.Dispose ();
				BoysPointsContainer = null;
			}

			if (GirlsPointsContainer != null) {
				GirlsPointsContainer.Dispose ();
				GirlsPointsContainer = null;
			}

		}
	}
}
