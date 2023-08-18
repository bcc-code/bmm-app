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
	[Register ("ExternalRelationsPlayTableViewCell")]
	partial class ExternalRelationsPlayTableViewCell
	{
		[Outlet]
		UIKit.UIView AnimationView { get; set; }

		[Outlet]
		UIKit.UIView PlayButton { get; set; }

		[Outlet]
		UIKit.UIImageView PlayIcon { get; set; }

		[Outlet]
		UIKit.UILabel PlayLabel { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (PlayButton != null) {
				PlayButton.Dispose ();
				PlayButton = null;
			}

			if (PlayLabel != null) {
				PlayLabel.Dispose ();
				PlayLabel = null;
			}

			if (AnimationView != null) {
				AnimationView.Dispose ();
				AnimationView = null;
			}

			if (PlayIcon != null) {
				PlayIcon.Dispose ();
				PlayIcon = null;
			}

		}
	}
}
