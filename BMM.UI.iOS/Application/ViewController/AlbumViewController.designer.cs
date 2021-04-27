// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using FFImageLoading.Cross;
using UIKit;

namespace BMM.UI.iOS
{
    [Register ("AlbumViewController")]
    partial class AlbumViewController
    {
        [Outlet]
        MvxCachedImageView AlbumBlurCoverImage { get; set; }


        [Outlet]
        MvxCachedImageView AlbumCoverImageView { get; set; }


        [Outlet]
        UIKit.UIView AlbumHeaderView { get; set; }


        [Outlet]
        UIKit.UITableView AlbumTable { get; set; }


        [Outlet]
        UIKit.UIView blurView { get; set; }


        [Outlet]
        UIKit.UIButton ShuffleButton { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (AlbumBlurCoverImage != null) {
                AlbumBlurCoverImage.Dispose ();
                AlbumBlurCoverImage = null;
            }

            if (AlbumCoverImageView != null) {
                AlbumCoverImageView.Dispose ();
                AlbumCoverImageView = null;
            }

            if (AlbumHeaderView != null) {
                AlbumHeaderView.Dispose ();
                AlbumHeaderView = null;
            }

            if (AlbumTable != null) {
                AlbumTable.Dispose ();
                AlbumTable = null;
            }

            if (blurView != null) {
                blurView.Dispose ();
                blurView = null;
            }

            if (ShuffleButton != null) {
                ShuffleButton.Dispose ();
                ShuffleButton = null;
            }
        }
    }
}