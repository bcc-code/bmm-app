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
	[Register ("QuizQuestionViewController")]
	partial class QuizQuestionViewController
	{
		[Outlet]
		UIKit.UIStackView AnswersStackView { get; set; }

		[Outlet]
		UIKit.UIImageView BackgroundImage { get; set; }

		[Outlet]
		UIKit.UIView CloseIconView { get; set; }

		[Outlet]
		UIKit.UIStackView QuestionsStackView { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (AnswersStackView != null) {
				AnswersStackView.Dispose ();
				AnswersStackView = null;
			}

			if (CloseIconView != null) {
				CloseIconView.Dispose ();
				CloseIconView = null;
			}

			if (QuestionsStackView != null) {
				QuestionsStackView.Dispose ();
				QuestionsStackView = null;
			}

			if (BackgroundImage != null) {
				BackgroundImage.Dispose ();
				BackgroundImage = null;
			}

		}
	}
}
