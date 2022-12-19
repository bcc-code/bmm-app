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
	[Register ("SearchViewController")]
	partial class SearchViewController
	{
		[Outlet]
		UIKit.UIButton CancelButton { get; set; }

		[Outlet]
		UIKit.UIView ClearHistoryButton { get; set; }

		[Outlet]
		UIKit.UIView ContainerView { get; set; }

		[Outlet]
		UIKit.UILabel RecentSearchesLabel { get; set; }

		[Outlet]
		UIKit.UIView RecentSearchesLayer { get; set; }

		[Outlet]
		UIKit.UITableView RecentSearchesTableView { get; set; }

		[Outlet]
		UIKit.UIView SearchBarBottomSeparator { get; set; }

		[Outlet]
		BMM.UI.iOS.CustomViews.BmmSeachTextField SearchTextField { get; set; }

		[Outlet]
		UIKit.UIView WelcomeLayer { get; set; }

		[Outlet]
		UIKit.UILabel WelcomeSubtitleLabel { get; set; }

		[Outlet]
		UIKit.UILabel WelcomeTitleLabel { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (CancelButton != null) {
				CancelButton.Dispose ();
				CancelButton = null;
			}

			if (ClearHistoryButton != null) {
				ClearHistoryButton.Dispose ();
				ClearHistoryButton = null;
			}

			if (ContainerView != null) {
				ContainerView.Dispose ();
				ContainerView = null;
			}

			if (RecentSearchesLabel != null) {
				RecentSearchesLabel.Dispose ();
				RecentSearchesLabel = null;
			}

			if (RecentSearchesLayer != null) {
				RecentSearchesLayer.Dispose ();
				RecentSearchesLayer = null;
			}

			if (RecentSearchesTableView != null) {
				RecentSearchesTableView.Dispose ();
				RecentSearchesTableView = null;
			}

			if (SearchTextField != null) {
				SearchTextField.Dispose ();
				SearchTextField = null;
			}

			if (WelcomeLayer != null) {
				WelcomeLayer.Dispose ();
				WelcomeLayer = null;
			}

			if (WelcomeSubtitleLabel != null) {
				WelcomeSubtitleLabel.Dispose ();
				WelcomeSubtitleLabel = null;
			}

			if (WelcomeTitleLabel != null) {
				WelcomeTitleLabel.Dispose ();
				WelcomeTitleLabel = null;
			}

			if (SearchBarBottomSeparator != null) {
				SearchBarBottomSeparator.Dispose ();
				SearchBarBottomSeparator = null;
			}

		}
	}
}
