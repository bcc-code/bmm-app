﻿// WARNING
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
	[Register ("ProjectBoxAchievementsCollectionViewCell")]
	partial class ProjectBoxAchievementsCollectionViewCell
	{
		[Outlet]
		FFImageLoading.Cross.MvxCachedImageView ImageView { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (ImageView != null) {
				ImageView.Dispose ();
				ImageView = null;
			}

		}
	}
}
