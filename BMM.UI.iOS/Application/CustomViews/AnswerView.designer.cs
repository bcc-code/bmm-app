// WARNING
//
// This file has been generated automatically by Rider IDE
//   to store outlets and actions made in Xcode.
// If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace BMM.UI.iOS.CustomViews
{
	[Register ("AnswerView")]
	partial class AnswerView
	{
		[Outlet]
		UIKit.UILabel AnswerLabel { get; set; }

		[Outlet]
		UIKit.UILabel AnswerLetterLabel { get; set; }

		[Outlet]
		UIKit.UIView ContainerView { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (AnswerLabel != null) {
				AnswerLabel.Dispose ();
				AnswerLabel = null;
			}

			if (AnswerLetterLabel != null) {
				AnswerLetterLabel.Dispose ();
				AnswerLetterLabel = null;
			}

			if (ContainerView != null) {
				ContainerView.Dispose ();
				ContainerView = null;
			}

		}
	}
}
