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
	[Register ("SettingsViewController")]
	partial class SettingsViewController
	{
		[Outlet]
		BMM.UI.iOS.CustomViews.BmmFormattedLabel FormattedText { get; set; }

		[Outlet]
		UIKit.UITableView SettingsTableView { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (SettingsTableView != null) {
				SettingsTableView.Dispose ();
				SettingsTableView = null;
			}

			if (FormattedText != null) {
				FormattedText.Dispose ();
				FormattedText = null;
			}

		}
	}
}
