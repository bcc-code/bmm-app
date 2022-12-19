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
		UIKit.UITableView ResultsTableView { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (NoResultsMessage != null) {
				NoResultsMessage.Dispose ();
				NoResultsMessage = null;
			}

			if (NoResultsTitle != null) {
				NoResultsTitle.Dispose ();
				NoResultsTitle = null;
			}

			if (ResultsTableView != null) {
				ResultsTableView.Dispose ();
				ResultsTableView = null;
			}

			if (ActivityIndicator != null) {
				ActivityIndicator.Dispose ();
				ActivityIndicator = null;
			}

			if (NoItemsLayer != null) {
				NoItemsLayer.Dispose ();
				NoItemsLayer = null;
			}

		}
	}
}
