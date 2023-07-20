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
	[Register ("BibleStudyHeaderTableViewCell")]
	partial class BibleStudyHeaderTableViewCell
	{
		[Outlet]
		UIKit.UILabel AudiobookLabel { get; set; }

		[Outlet]
		BMM.UI.iOS.Separator Divider { get; set; }

		[Outlet]
		UIKit.UILabel EpisodeDateLabel { get; set; }

		[Outlet]
		UIKit.UILabel EpisodeTitleLabel { get; set; }

		[Outlet]
		UIKit.UIView ListenToAudiobookView { get; set; }

		[Outlet]
		UIKit.UIView PlayButtonView { get; set; }

		[Outlet]
		UIKit.UILabel PlayLabel { get; set; }

		[Outlet]
		UIKit.UILabel ThemeNameLabel { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (Divider != null) {
				Divider.Dispose ();
				Divider = null;
			}

			if (ThemeNameLabel != null) {
				ThemeNameLabel.Dispose ();
				ThemeNameLabel = null;
			}

			if (EpisodeTitleLabel != null) {
				EpisodeTitleLabel.Dispose ();
				EpisodeTitleLabel = null;
			}

			if (EpisodeDateLabel != null) {
				EpisodeDateLabel.Dispose ();
				EpisodeDateLabel = null;
			}

			if (PlayButtonView != null) {
				PlayButtonView.Dispose ();
				PlayButtonView = null;
			}

			if (ListenToAudiobookView != null) {
				ListenToAudiobookView.Dispose ();
				ListenToAudiobookView = null;
			}

			if (PlayLabel != null) {
				PlayLabel.Dispose ();
				PlayLabel = null;
			}

			if (AudiobookLabel != null) {
				AudiobookLabel.Dispose ();
				AudiobookLabel = null;
			}

		}
	}
}
