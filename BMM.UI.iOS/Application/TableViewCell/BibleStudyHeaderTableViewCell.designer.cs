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
	[Register ("BibleStudyHeaderTableViewCell")]
	partial class BibleStudyHeaderTableViewCell
	{
		[Outlet]
		UIKit.UILabel EpisodeDateLabel { get; set; }

		[Outlet]
		UIKit.UILabel EpisodeTitleLabel { get; set; }

		[Outlet]
		UIKit.UILabel ThemeNameLabel { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (EpisodeDateLabel != null) {
				EpisodeDateLabel.Dispose ();
				EpisodeDateLabel = null;
			}

			if (EpisodeTitleLabel != null) {
				EpisodeTitleLabel.Dispose ();
				EpisodeTitleLabel = null;
			}

			if (ThemeNameLabel != null) {
				ThemeNameLabel.Dispose ();
				ThemeNameLabel = null;
			}

		}
	}
}
