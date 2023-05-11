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
	[Register ("SearchResultsViewController")]
	partial class SearchResultsViewController
	{
		[Outlet]
		UIKit.UIActivityIndicatorView ActivityIndicator { get; set; }

		[Outlet]
		UIKit.UIView NoItemsLayer { get; set; }

		[Outlet]
		UIKit.UILabel NoResultsMessage { get; set; }

		[Outlet]
		UIKit.UILabel NoResultsTitle { get; set; }

		[Outlet]
		UIKit.UIButton ReloadButton { get; set; }

		[Outlet]
		UIKit.UIView ResultsContainer { get; set; }

		[Outlet]
		UIKit.UILabel ResultsLabel { get; set; }

		[Outlet]
		UIKit.UITableView ResultsTableView { get; set; }

		[Outlet]
		UIKit.UIView SearchFailedLayer { get; set; }

		[Outlet]
		UIKit.UILabel SearchFailedMessage { get; set; }

		[Outlet]
		UIKit.UILabel SearchFailedTitle { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (ActivityIndicator != null) {
				ActivityIndicator.Dispose ();
				ActivityIndicator = null;
			}

			if (NoItemsLayer != null) {
				NoItemsLayer.Dispose ();
				NoItemsLayer = null;
			}

			if (NoResultsMessage != null) {
				NoResultsMessage.Dispose ();
				NoResultsMessage = null;
			}

			if (NoResultsTitle != null) {
				NoResultsTitle.Dispose ();
				NoResultsTitle = null;
			}

			if (ResultsContainer != null) {
				ResultsContainer.Dispose ();
				ResultsContainer = null;
			}

			if (ResultsLabel != null) {
				ResultsLabel.Dispose ();
				ResultsLabel = null;
			}

			if (ResultsTableView != null) {
				ResultsTableView.Dispose ();
				ResultsTableView = null;
			}

			if (SearchFailedTitle != null) {
				SearchFailedTitle.Dispose ();
				SearchFailedTitle = null;
			}

			if (SearchFailedMessage != null) {
				SearchFailedMessage.Dispose ();
				SearchFailedMessage = null;
			}

			if (SearchFailedLayer != null) {
				SearchFailedLayer.Dispose ();
				SearchFailedLayer = null;
			}

			if (ReloadButton != null) {
				ReloadButton.Dispose ();
				ReloadButton = null;
			}

		}
	}
}
