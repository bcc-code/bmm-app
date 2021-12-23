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
	[Register ("FraKaareTableViewCell")]
	partial class FraKaareTableViewCell
	{
		[Outlet]
		UIKit.UIButton FraKaarePlayRandomButton { get; set; }

		[Outlet]
		UIKit.UIButton PodcastShowAllButton { get; set; }

		[Outlet]
		UIKit.UILabel PodcastTitleLabel { get; set; }

		[Outlet]
		UIKit.UITableView PodcastTrackListTableView { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (PodcastTrackListTableView != null) {
				PodcastTrackListTableView.Dispose ();
				PodcastTrackListTableView = null;
			}

			if (PodcastShowAllButton != null) {
				PodcastShowAllButton.Dispose ();
				PodcastShowAllButton = null;
			}

			if (PodcastTitleLabel != null) {
				PodcastTitleLabel.Dispose ();
				PodcastTitleLabel = null;
			}

			if (FraKaarePlayRandomButton != null) {
				FraKaarePlayRandomButton.Dispose ();
				FraKaarePlayRandomButton = null;
			}

		}
	}
}
