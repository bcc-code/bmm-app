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
	[Register ("ReadTranscriptionTableViewCell")]
	partial class ReadTranscriptionTableViewCell
	{
		[Outlet]
		UIKit.UILabel TranscriptionsTextLabel { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (TranscriptionsTextLabel != null) {
                TranscriptionsTextLabel.Dispose ();
                TranscriptionsTextLabel = null;
			}

		}
	}
}
