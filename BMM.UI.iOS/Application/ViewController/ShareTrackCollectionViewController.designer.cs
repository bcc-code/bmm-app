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
	[Register ("ShareTrackCollectionViewController")]
	partial class ShareTrackCollectionViewController
	{
		[Outlet]
		UIKit.UIButton MakePrivateButton { get; set; }

		[Outlet]
		UIKit.UILabel NoteLabel { get; set; }

		[Outlet]
		UIKit.UILabel PlaylistName { get; set; }

		[Outlet]
		UIKit.UILabel PlaylistType { get; set; }

		[Outlet]
		UIKit.UIButton ShareLinkButton { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (PlaylistName != null) {
				PlaylistName.Dispose ();
				PlaylistName = null;
			}

			if (PlaylistType != null) {
				PlaylistType.Dispose ();
				PlaylistType = null;
			}

			if (ShareLinkButton != null) {
				ShareLinkButton.Dispose ();
				ShareLinkButton = null;
			}

			if (NoteLabel != null) {
				NoteLabel.Dispose ();
				NoteLabel = null;
			}

			if (MakePrivateButton != null) {
				MakePrivateButton.Dispose ();
				MakePrivateButton = null;
			}

		}
	}
}
