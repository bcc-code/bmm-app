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
	[Register ("TopSongsCollectionViewController")]
	partial class TopSongsCollectionViewController
	{
		[Outlet]
		UIKit.UIButton AddToFavouritesButton { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint ButtonBottomConstraint { get; set; }

		[Outlet]
		UIKit.UITableView CollectionTableView { get; set; }

		[Outlet]
		UIKit.UILabel HeaderLabel { get; set; }

		[Outlet]
		UIKit.UILabel TrackCountLabel { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (HeaderLabel != null) {
				HeaderLabel.Dispose ();
				HeaderLabel = null;
			}

			if (AddToFavouritesButton != null) {
				AddToFavouritesButton.Dispose ();
				AddToFavouritesButton = null;
			}

			if (ButtonBottomConstraint != null) {
				ButtonBottomConstraint.Dispose ();
				ButtonBottomConstraint = null;
			}

			if (CollectionTableView != null) {
				CollectionTableView.Dispose ();
				CollectionTableView = null;
			}

			if (TrackCountLabel != null) {
				TrackCountLabel.Dispose ();
				TrackCountLabel = null;
			}

		}
	}
}
