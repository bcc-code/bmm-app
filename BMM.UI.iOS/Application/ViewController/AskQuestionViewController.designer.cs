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
	[Register ("AskQuestionViewController")]
	partial class AskQuestionViewController
	{
		[Outlet]
		UIKit.UIView BottomView { get; set; }

		[Outlet]
		UIKit.UIView CloseButton { get; set; }

		[Outlet]
		UIKit.UITextView QuestionTextView { get; set; }

		[Outlet]
		UIKit.UIButton SubmitButton { get; set; }

		[Outlet]
		UIKit.UILabel TitleLabel { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (BottomView != null) {
				BottomView.Dispose ();
				BottomView = null;
			}

			if (SubmitButton != null) {
				SubmitButton.Dispose ();
				SubmitButton = null;
			}

			if (TitleLabel != null) {
				TitleLabel.Dispose ();
				TitleLabel = null;
			}

			if (QuestionTextView != null) {
				QuestionTextView.Dispose ();
				QuestionTextView = null;
			}

			if (CloseButton != null) {
				CloseButton.Dispose ();
				CloseButton = null;
			}

		}
	}
}
