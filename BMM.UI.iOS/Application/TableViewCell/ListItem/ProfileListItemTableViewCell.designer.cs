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
	[Register ("ProfileListItemTableViewCell")]
	partial class ProfileListItemTableViewCell
	{
		[Outlet]
		UIKit.UIButton AchievementsButton { get; set; }

		[Outlet]
		FFImageLoading.Cross.MvxCachedImageView ProfileImage { get; set; }

		[Outlet]
		UIKit.UILabel SignedInAsTitle { get; set; }

		[Outlet]
		UIKit.UIButton SignOutButton { get; set; }

		[Outlet]
		UIKit.UILabel Username { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (ProfileImage != null) {
				ProfileImage.Dispose ();
				ProfileImage = null;
			}

			if (SignedInAsTitle != null) {
				SignedInAsTitle.Dispose ();
				SignedInAsTitle = null;
			}

			if (SignOutButton != null) {
				SignOutButton.Dispose ();
				SignOutButton = null;
			}

			if (AchievementsButton != null) {
				AchievementsButton.Dispose ();
				AchievementsButton = null;
			}

			if (Username != null) {
				Username.Dispose ();
				Username = null;
			}

		}
	}
}
