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
    [Register ("ContributorViewController")]
    partial class ContributorViewController
    {
        [Outlet]
        UIKit.UIView blurView { get; set; }


        [Outlet]
        MvxCachedImageView CircleCoverImage { get; set; }


        [Outlet]
        MvxCachedImageView CoverImage { get; set; }


        [Outlet]
        UIKit.UITableView TracksTable { get; set; }


        [Outlet]
        UIKit.UIView TracksTableHeader { get; set; }


        [Outlet]
        UIKit.UILabel TrackTableTitle { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (blurView != null) {
                blurView.Dispose ();
                blurView = null;
            }

            if (CircleCoverImage != null) {
                CircleCoverImage.Dispose ();
                CircleCoverImage = null;
            }

            if (CoverImage != null) {
                CoverImage.Dispose ();
                CoverImage = null;
            }

            if (TracksTable != null) {
                TracksTable.Dispose ();
                TracksTable = null;
            }

            if (TrackTableTitle != null) {
                TrackTableTitle.Dispose ();
                TrackTableTitle = null;
            }
        }
    }
}