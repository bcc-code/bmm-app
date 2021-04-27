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
    [Register ("SearchViewController")]
    partial class SearchViewController
    {
        [Outlet]
        UIKit.UIButton ClearHistoryButton { get; set; }


        [Outlet]
        UIKit.UIView EmptyResultsView { get; set; }


        [Outlet]
        UIKit.UILabel HistoryHeaderLabel { get; set; }


        [Outlet]
        UIKit.UILabel NoResultsLabel { get; set; }


        [Outlet]
        UIKit.UILabel ResultsHeaderLabel { get; set; }


        [Outlet]
        UIKit.UIView SearchExecutedView { get; set; }


        [Outlet]
        UIKit.UITableView SearchHistoryTable { get; set; }


        [Outlet]
        UIKit.UITableView SearchResultsTable { get; set; }


        [Outlet]
        UIKit.UITableView SearchSuggestionTable { get; set; }


        [Outlet]
        UIKit.UILabel SuggestionsHeaderLabel { get; set; }


        [Outlet]
        UIKit.UILabel WelcomeSubTitleLabel { get; set; }


        [Outlet]
        UIKit.UILabel WelcomeTitleLabel { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (ClearHistoryButton != null) {
                ClearHistoryButton.Dispose ();
                ClearHistoryButton = null;
            }

            if (EmptyResultsView != null) {
                EmptyResultsView.Dispose ();
                EmptyResultsView = null;
            }

            if (HistoryHeaderLabel != null) {
                HistoryHeaderLabel.Dispose ();
                HistoryHeaderLabel = null;
            }

            if (NoResultsLabel != null) {
                NoResultsLabel.Dispose ();
                NoResultsLabel = null;
            }

            if (ResultsHeaderLabel != null) {
                ResultsHeaderLabel.Dispose ();
                ResultsHeaderLabel = null;
            }

            if (SearchExecutedView != null) {
                SearchExecutedView.Dispose ();
                SearchExecutedView = null;
            }

            if (SearchHistoryTable != null) {
                SearchHistoryTable.Dispose ();
                SearchHistoryTable = null;
            }

            if (SearchResultsTable != null) {
                SearchResultsTable.Dispose ();
                SearchResultsTable = null;
            }

            if (SearchSuggestionTable != null) {
                SearchSuggestionTable.Dispose ();
                SearchSuggestionTable = null;
            }

            if (SuggestionsHeaderLabel != null) {
                SuggestionsHeaderLabel.Dispose ();
                SuggestionsHeaderLabel = null;
            }

            if (WelcomeSubTitleLabel != null) {
                WelcomeSubTitleLabel.Dispose ();
                WelcomeSubTitleLabel = null;
            }

            if (WelcomeTitleLabel != null) {
                WelcomeTitleLabel.Dispose ();
                WelcomeTitleLabel = null;
            }
        }
    }
}