using System;
using BMM.Api.Implementation.Models;
using BMM.Core.Helpers.Interfaces;
using BMM.Core.Models.POs.Base;
using BMM.Core.Models.POs.Base.Interfaces;
using BMM.Core.ViewModels;
using Foundation;
using MvvmCross.Commands;
using MvvmCross.ViewModels;
using UIKit;

namespace BMM.UI.iOS
{
    public class LoadMoreTableViewSource : BaseTableViewSource
    {
        public LoadMoreTableViewSource (UITableView tableView)
            : base(tableView)
        {
            var nibName = LoadingTableViewCell.Key;
            tableView.RegisterNibForCellReuse(UINib.FromName(nibName, NSBundle.MainBundle), nibName);
            ShowCellLoadingSpinner = true;
        }

        public IMvxCommand LoadMoreCommand { get; set; }

        private bool isFullyLoaded = true;
        
        public bool ShowCellLoadingSpinner { get; set; }

        public bool IsFullyLoaded {
            get {
                return isFullyLoaded;
            }
            set {
                isFullyLoaded = value;
                InvokeOnMainThread (delegate {
                    TableView.ReloadData ();
                });
            }
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var data = (IBmmObservableCollection<IDocumentPO>)ItemsSource;

            UITableViewCell cell;
            if (data != null && indexPath.Row == data.Count)
            {
                cell = tableView.DequeueReusableCell (LoadingTableViewCell.Key);
                (cell as LoadingTableViewCell)?.AnimateSpinner();
            }
            else
            {
                cell = base.GetCell (tableView, indexPath);

                UIView backgroundColorView = new UIView ();
                backgroundColorView.BackgroundColor = UIColor.FromRGB (239, 242, 245);
                cell.SelectedBackgroundView = backgroundColorView;
            }

            return cell;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            var data = (IBmmObservableCollection<IDocumentPO>)ItemsSource;
            
            if (data != null && !IsFullyLoaded && ShowCellLoadingSpinner) {
                // Add one row here for the loading-indicator.
                return base.RowsInSection (tableview, section) + 1;
            } else {
                return base.RowsInSection (tableview, section);
            }
        }
        
        public override void WillDisplay(UITableView tableView, UITableViewCell cell, NSIndexPath indexPath)
        {
            base.WillDisplay(tableView, cell, indexPath);
            var data = (IBmmObservableCollection<IDocumentPO>)ItemsSource;

            if (!IsFullyLoaded && LoadMoreCommand != null && indexPath.Row == data.Count - 1) {
                LoadMoreCommand.Execute();
            }
        }
    }
}

