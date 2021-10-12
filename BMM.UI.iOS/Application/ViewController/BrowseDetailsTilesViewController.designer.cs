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
	[Register ("BrowseDetailsTilesViewController")]
	partial class BrowseDetailsTilesViewController
	{
		[Outlet]
		UIKit.UICollectionView DocumentsTilesCollectionView { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (DocumentsTilesCollectionView != null) {
				DocumentsTilesCollectionView.Dispose ();
				DocumentsTilesCollectionView = null;
			}

		}
	}
}
