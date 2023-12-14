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
	[Register ("ProjectBoxExpandedViewCell")]
	partial class ProjectBoxExpandedViewCell
	{
		[Outlet]
		UIKit.UICollectionView AchievementsCollectionView { get; set; }

		[Outlet]
		UIKit.UIView AchievementsContainer { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint AchievementsHeightConstraint { get; set; }

		[Outlet]
		UIKit.UIStackView AchievementStackView { get; set; }

		[Outlet]
		UIKit.UIButton CollapseButton { get; set; }

		[Outlet]
		UIKit.UILabel PointsLabel { get; set; }

		[Outlet]
		UIKit.UILabel PointsNumber { get; set; }

		[Outlet]
		BMM.UI.iOS.LeftImageButton QuestionsButton { get; set; }

		[Outlet]
		UIKit.UIView RulesContainer { get; set; }

		[Outlet]
		UIKit.UILabel RulesLabel { get; set; }

		[Outlet]
		UIKit.UILabel TitleLabel { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (AchievementsCollectionView != null) {
				AchievementsCollectionView.Dispose ();
				AchievementsCollectionView = null;
			}

			if (CollapseButton != null) {
				CollapseButton.Dispose ();
				CollapseButton = null;
			}

			if (PointsLabel != null) {
				PointsLabel.Dispose ();
				PointsLabel = null;
			}

			if (PointsNumber != null) {
				PointsNumber.Dispose ();
				PointsNumber = null;
			}

			if (QuestionsButton != null) {
				QuestionsButton.Dispose ();
				QuestionsButton = null;
			}

			if (RulesContainer != null) {
				RulesContainer.Dispose ();
				RulesContainer = null;
			}

			if (RulesLabel != null) {
				RulesLabel.Dispose ();
				RulesLabel = null;
			}

			if (TitleLabel != null) {
				TitleLabel.Dispose ();
				TitleLabel = null;
			}

			if (AchievementsContainer != null) {
				AchievementsContainer.Dispose ();
				AchievementsContainer = null;
			}

			if (AchievementsHeightConstraint != null) {
				AchievementsHeightConstraint.Dispose ();
				AchievementsHeightConstraint = null;
			}

			if (AchievementStackView != null) {
				AchievementStackView.Dispose ();
				AchievementStackView = null;
			}

		}
	}
}
