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
	[Register ("SearchSuggestionTableViewCell")]
	partial class SearchSuggestionTableViewCell
	{
		[Outlet]
		UIKit.UILabel TextLabel { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (TextLabel != null) {
				TextLabel.Dispose ();
				TextLabel = null;
			}

		}
	}
}
