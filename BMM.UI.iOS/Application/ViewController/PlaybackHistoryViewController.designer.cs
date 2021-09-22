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
	[Register ("PlaybackHistoryViewController")]
	partial class PlaybackHistoryViewController
	{
		[Outlet]
		UIKit.UILabel NoHistoryLabel { get; set; }

		[Outlet]
		UIKit.UITableView PlaybackHistoryTableView { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (PlaybackHistoryTableView != null) {
				PlaybackHistoryTableView.Dispose ();
				PlaybackHistoryTableView = null;
			}

			if (NoHistoryLabel != null) {
				NoHistoryLabel.Dispose ();
				NoHistoryLabel = null;
			}

		}
	}
}
