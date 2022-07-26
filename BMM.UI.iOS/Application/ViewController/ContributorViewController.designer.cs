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
	[Register ("ContributorViewController")]
	partial class ContributorViewController
	{
		[Outlet]
		FFImageLoading.Cross.MvxCachedImageView CircleCoverImage { get; set; }

		[Outlet]
		UIKit.UILabel NameLabel { get; set; }

		[Outlet]
		UIKit.UIButton ShuffleButton { get; set; }

		[Outlet]
		UIKit.UILabel TrackCountLabel { get; set; }

		[Outlet]
		UIKit.UITableView TracksTable { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (CircleCoverImage != null) {
				CircleCoverImage.Dispose ();
				CircleCoverImage = null;
			}

			if (NameLabel != null) {
				NameLabel.Dispose ();
				NameLabel = null;
			}

			if (ShuffleButton != null) {
				ShuffleButton.Dispose ();
				ShuffleButton = null;
			}

			if (TracksTable != null) {
				TracksTable.Dispose ();
				TracksTable = null;
			}

			if (TrackCountLabel != null) {
				TrackCountLabel.Dispose ();
				TrackCountLabel = null;
			}

		}
	}
}
