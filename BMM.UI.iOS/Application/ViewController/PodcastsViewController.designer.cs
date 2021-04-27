// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace BMM.UI.iOS
{
    [Register ("PodcastsViewController")]
    partial class PodcastsViewController
    {
        [Outlet]
        UIKit.UICollectionView PodcastCollectionView { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (PodcastCollectionView != null) {
                PodcastCollectionView.Dispose ();
                PodcastCollectionView = null;
            }
        }
    }
}