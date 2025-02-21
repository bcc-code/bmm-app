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
	[Register ("HighlightedChurchTableViewCell")]
	partial class HighlightedChurchTableViewCell
	{
		[Outlet]
		UIKit.UILabel BoysLabel { get; set; }

		[Outlet]
		UIKit.UIView BoysPointsContainer { get; set; }

		[Outlet]
		UIKit.UILabel BoysPointsLabel { get; set; }

		[Outlet]
		UIKit.UILabel ChurchName { get; set; }

		[Outlet]
		UIKit.UILabel GirlsLabel { get; set; }

		[Outlet]
		UIKit.UIView GirlsPointsContainer { get; set; }

		[Outlet]
		UIKit.UILabel GirlsPointsLabel { get; set; }

		[Outlet]
		UIKit.UIView LeftPointsContainer { get; set; }

		[Outlet]
		UIKit.UILabel LeftPointsLabel { get; set; }

		[Outlet]
		UIKit.UIView MiddlePointsContainer { get; set; }

		[Outlet]
		UIKit.UILabel MiddlePointsLabel { get; set; }

		[Outlet]
		UIKit.UIView RightPointsContainer { get; set; }

		[Outlet]
		UIKit.UILabel RightPointsLabel { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (ChurchName != null) {
				ChurchName.Dispose ();
				ChurchName = null;
			}

			if (RightPointsContainer != null) {
				RightPointsContainer.Dispose ();
				RightPointsContainer = null;
			}

			if (MiddlePointsContainer != null) {
				MiddlePointsContainer.Dispose ();
				MiddlePointsContainer = null;
			}

			if (LeftPointsContainer != null) {
				LeftPointsContainer.Dispose ();
				LeftPointsContainer = null;
			}

			if (RightPointsLabel != null) {
				RightPointsLabel.Dispose ();
				RightPointsLabel = null;
			}

			if (MiddlePointsLabel != null) {
				MiddlePointsLabel.Dispose ();
				MiddlePointsLabel = null;
			}

			if (LeftPointsLabel != null) {
				LeftPointsLabel.Dispose ();
				LeftPointsLabel = null;
			}

			if (BoysPointsContainer != null) {
				BoysPointsContainer.Dispose ();
				BoysPointsContainer = null;
			}

			if (GirlsPointsContainer != null) {
				GirlsPointsContainer.Dispose ();
				GirlsPointsContainer = null;
			}

			if (BoysLabel != null) {
				BoysLabel.Dispose ();
				BoysLabel = null;
			}

			if (GirlsLabel != null) {
				GirlsLabel.Dispose ();
				GirlsLabel = null;
			}

			if (GirlsPointsLabel != null) {
				GirlsPointsLabel.Dispose ();
				GirlsPointsLabel = null;
			}

			if (BoysPointsLabel != null) {
				BoysPointsLabel.Dispose ();
				BoysPointsLabel = null;
			}

		}
	}
}
