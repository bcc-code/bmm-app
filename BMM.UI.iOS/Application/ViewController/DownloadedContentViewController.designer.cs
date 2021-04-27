// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace BMM.UI.iOS
{
    [Register ("DownloadedContentViewController")]
    partial class DownloadedContentViewController
    {
        [Outlet]
        UIKit.UITableView MyCollectionTable { get; set; }


        [Outlet]
        UIKit.UIView NoOfflineTrackCollectionView { get; set; }


        [Outlet]
        UIKit.UILabel NoOfflineTrackCollectionHeadlineLabel { get; set; }


        [Outlet]
        UIKit.UILabel NoOfflineTrackCollectionTextLabel { get; set; }

        [Outlet]
        UIKit.UILabel OfflineBannerLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView OfflineBannerView { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (MyCollectionTable != null) {
                MyCollectionTable.Dispose ();
                MyCollectionTable = null;
            }

            if (NoOfflineTrackCollectionHeadlineLabel != null) {
                NoOfflineTrackCollectionHeadlineLabel.Dispose ();
                NoOfflineTrackCollectionHeadlineLabel = null;
            }

            if (NoOfflineTrackCollectionTextLabel != null) {
                NoOfflineTrackCollectionTextLabel.Dispose ();
                NoOfflineTrackCollectionTextLabel = null;
            }

            if (NoOfflineTrackCollectionView != null) {
                NoOfflineTrackCollectionView.Dispose ();
                NoOfflineTrackCollectionView = null;
            }

            if (OfflineBannerLabel != null) {
                OfflineBannerLabel.Dispose ();
                OfflineBannerLabel = null;
            }

            if (OfflineBannerView != null) {
                OfflineBannerView.Dispose ();
                OfflineBannerView = null;
            }
        }
    }
}