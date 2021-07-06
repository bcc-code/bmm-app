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
	[Register ("SharedTrackCollectionViewController")]
	partial class SharedTrackCollectionViewController
	{
		[Outlet]
		UIKit.UIButton AddToMyPlaylistButton { get; set; }

		[Outlet]
		UIKit.UILabel AuthorNameLabel { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint ButtonBottomConstraint { get; set; }

		[Outlet]
		UIKit.UITableView CollectionTableView { get; set; }

		[Outlet]
		UIKit.UILabel PlaylistName { get; set; }

		[Outlet]
		UIKit.UILabel TrackCountLabel { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (AuthorNameLabel != null) {
				AuthorNameLabel.Dispose ();
				AuthorNameLabel = null;
			}

			if (CollectionTableView != null) {
				CollectionTableView.Dispose ();
				CollectionTableView = null;
			}

			if (PlaylistName != null) {
				PlaylistName.Dispose ();
				PlaylistName = null;
			}

			if (TrackCountLabel != null) {
				TrackCountLabel.Dispose ();
				TrackCountLabel = null;
			}

			if (AddToMyPlaylistButton != null) {
				AddToMyPlaylistButton.Dispose ();
				AddToMyPlaylistButton = null;
			}

			if (ButtonBottomConstraint != null) {
				ButtonBottomConstraint.Dispose ();
				ButtonBottomConstraint = null;
			}

		}
	}
}
