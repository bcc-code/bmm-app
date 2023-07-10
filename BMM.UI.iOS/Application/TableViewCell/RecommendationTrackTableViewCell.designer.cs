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
	[Register ("RecommendationTrackTableViewCell")]
	partial class RecommendationTrackTableViewCell
	{
		[Outlet]
		UIKit.UILabel RemoteSubtitleLabel { get; set; }

		[Outlet]
		UIKit.UIView RemoteTitleContainer { get; set; }

		[Outlet]
		UIKit.UILabel RemoteTitleLabel { get; set; }

		[Outlet]
		UIKit.UILabel TitleLabel { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint TitleToBottomViewConstraint { get; set; }

		[Outlet]
		UIKit.UILabel TrackMetaLabel { get; set; }

		[Outlet]
		UIKit.UILabel TrackSubtitleLabel { get; set; }

		[Outlet]
		UIKit.UILabel TrackTitleLabel { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (RemoteSubtitleLabel != null) {
				RemoteSubtitleLabel.Dispose ();
				RemoteSubtitleLabel = null;
			}

			if (RemoteTitleLabel != null) {
				RemoteTitleLabel.Dispose ();
				RemoteTitleLabel = null;
			}

			if (TitleLabel != null) {
				TitleLabel.Dispose ();
				TitleLabel = null;
			}

			if (TitleToBottomViewConstraint != null) {
				TitleToBottomViewConstraint.Dispose ();
				TitleToBottomViewConstraint = null;
			}

			if (TrackMetaLabel != null) {
				TrackMetaLabel.Dispose ();
				TrackMetaLabel = null;
			}

			if (TrackSubtitleLabel != null) {
				TrackSubtitleLabel.Dispose ();
				TrackSubtitleLabel = null;
			}

			if (TrackTitleLabel != null) {
				TrackTitleLabel.Dispose ();
				TrackTitleLabel = null;
			}

			if (RemoteTitleContainer != null) {
				RemoteTitleContainer.Dispose ();
				RemoteTitleContainer = null;
			}

		}
	}
}
