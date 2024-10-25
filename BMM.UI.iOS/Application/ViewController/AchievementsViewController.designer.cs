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
	[Register ("AchievementsViewController")]
	partial class AchievementsViewController
	{
		[Outlet]
		UIKit.UICollectionView AchievementsCollectionView { get; set; }

		[Outlet]
		UIKit.UIStackView NoAchievementsStackView { get; set; }

		[Outlet]
		UIKit.UILabel NoAchievementsSubtitle { get; set; }

		[Outlet]
		UIKit.UILabel NoAchievementsTitle { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (AchievementsCollectionView != null) {
				AchievementsCollectionView.Dispose ();
				AchievementsCollectionView = null;
			}

			if (NoAchievementsTitle != null) {
				NoAchievementsTitle.Dispose ();
				NoAchievementsTitle = null;
			}

			if (NoAchievementsSubtitle != null) {
				NoAchievementsSubtitle.Dispose ();
				NoAchievementsSubtitle = null;
			}

			if (NoAchievementsStackView != null) {
				NoAchievementsStackView.Dispose ();
				NoAchievementsStackView = null;
			}

		}
	}
}
