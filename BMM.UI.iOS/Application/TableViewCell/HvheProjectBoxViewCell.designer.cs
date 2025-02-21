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
	[Register ("HvheProjectBoxViewCell")]
	partial class HvheProjectBoxViewCell
	{
		[Outlet]
		UIKit.UICollectionView AchievementsCollectionView { get; set; }

		[Outlet]
		UIKit.UIView BoysPointsContainer { get; set; }

		[Outlet]
		UIKit.UILabel BoysPointsLabel { get; set; }

		[Outlet]
		UIKit.UIView GirlsPointsContainer { get; set; }

		[Outlet]
		UIKit.UILabel GirlsPointsLabel { get; set; }

		[Outlet]
		UIKit.UILabel PointsLabel { get; set; }

		[Outlet]
		UIKit.UIView RulesContainer { get; set; }

		[Outlet]
		UIKit.UILabel RulesLabel { get; set; }

		[Outlet]
		UIKit.UILabel TitleLabel { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (BoysPointsContainer != null) {
				BoysPointsContainer.Dispose ();
				BoysPointsContainer = null;
			}

			if (BoysPointsLabel != null) {
				BoysPointsLabel.Dispose ();
				BoysPointsLabel = null;
			}

			if (GirlsPointsContainer != null) {
				GirlsPointsContainer.Dispose ();
				GirlsPointsContainer = null;
			}

			if (GirlsPointsLabel != null) {
				GirlsPointsLabel.Dispose ();
				GirlsPointsLabel = null;
			}

			if (PointsLabel != null) {
				PointsLabel.Dispose ();
				PointsLabel = null;
			}

			if (RulesContainer != null) {
				RulesContainer.Dispose ();
				RulesContainer = null;
			}

			if (RulesLabel != null) {
				RulesLabel.Dispose ();
				RulesLabel = null;
			}

			if (AchievementsCollectionView != null) {
				AchievementsCollectionView.Dispose ();
				AchievementsCollectionView = null;
			}

			if (TitleLabel != null) {
				TitleLabel.Dispose ();
				TitleLabel = null;
			}

		}
	}
}
