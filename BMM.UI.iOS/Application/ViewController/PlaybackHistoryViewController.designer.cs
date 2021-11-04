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
		UIKit.UILabel NoEntriesLabelSubtitle { get; set; }

		[Outlet]
		UIKit.UILabel NoEntriesLabelTitle { get; set; }

		[Outlet]
		UIKit.UITableView PlaybackHistoryTableView { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (PlaybackHistoryTableView != null) {
				PlaybackHistoryTableView.Dispose ();
				PlaybackHistoryTableView = null;
			}

			if (NoEntriesLabelTitle != null) {
				NoEntriesLabelTitle.Dispose ();
				NoEntriesLabelTitle = null;
			}

			if (NoEntriesLabelSubtitle != null) {
				NoEntriesLabelSubtitle.Dispose ();
				NoEntriesLabelSubtitle = null;
			}

		}
	}
}
