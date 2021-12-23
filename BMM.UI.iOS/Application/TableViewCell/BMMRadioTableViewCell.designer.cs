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
	[Register ("BMMRadioTableViewCell")]
	partial class BMMRadioTableViewCell
	{
		[Outlet]
		UIKit.UILabel BmmRadioBroadcastingDescription { get; set; }

		[Outlet]
		UIKit.UILabel BmmRadioBroadcastingTitle { get; set; }

		[Outlet]
		UIKit.UIView BmmRadioBroadcastingView { get; set; }

		[Outlet]
		UIKit.UIButton BmmRadioPlay { get; set; }

		[Outlet]
		UIKit.UILabel BmmRadioUpcomingDescription { get; set; }

		[Outlet]
		UIKit.UIButton BmmRadioUpcomingPlay { get; set; }

		[Outlet]
		UIKit.UILabel BmmRadioUpcomingTitle { get; set; }

		[Outlet]
		UIKit.UIView BmmRadioUpcomingView { get; set; }

		[Outlet]
		UIKit.UILabel CountdownLabel { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (BmmRadioBroadcastingView != null) {
				BmmRadioBroadcastingView.Dispose ();
				BmmRadioBroadcastingView = null;
			}

			if (BmmRadioUpcomingView != null) {
				BmmRadioUpcomingView.Dispose ();
				BmmRadioUpcomingView = null;
			}

			if (BmmRadioUpcomingPlay != null) {
				BmmRadioUpcomingPlay.Dispose ();
				BmmRadioUpcomingPlay = null;
			}

			if (BmmRadioPlay != null) {
				BmmRadioPlay.Dispose ();
				BmmRadioPlay = null;
			}

			if (BmmRadioBroadcastingTitle != null) {
				BmmRadioBroadcastingTitle.Dispose ();
				BmmRadioBroadcastingTitle = null;
			}

			if (BmmRadioBroadcastingDescription != null) {
				BmmRadioBroadcastingDescription.Dispose ();
				BmmRadioBroadcastingDescription = null;
			}

			if (BmmRadioUpcomingTitle != null) {
				BmmRadioUpcomingTitle.Dispose ();
				BmmRadioUpcomingTitle = null;
			}

			if (BmmRadioUpcomingDescription != null) {
				BmmRadioUpcomingDescription.Dispose ();
				BmmRadioUpcomingDescription = null;
			}

			if (CountdownLabel != null) {
				CountdownLabel.Dispose ();
				CountdownLabel = null;
			}

		}
	}
}
