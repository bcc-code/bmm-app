using MvvmCross.Platforms.Ios.Binding.Views;
using Foundation;
using System;
using System.Linq;
using MvvmCross.Commands;
using UIKit;

namespace BMM.UI.iOS
{
    public class SimpleLoadingTableViewSource : MvxSimpleTableViewSource
    {
        public IMvxCommand LoadMoreCommand { get; set; }

        private bool isFullyLoaded = true;

        public bool IsFullyLoaded
        {
            get
            {
                return isFullyLoaded;
            }
            set
            {
                isFullyLoaded = value;
                InvokeOnMainThread(delegate
                {
                    TableView.ReloadData();
                });
            }
        }

        public SimpleLoadingTableViewSource(UITableView tableView, string nibName, string cellIdentifier = null, NSBundle bundle = null) : base(tableView, nibName, cellIdentifier, bundle)
        {
            tableView.RegisterNibForCellReuse(UINib.FromName(LoadingTableViewCell.Key, NSBundle.MainBundle), LoadingTableViewCell.Key);
        }

        public SimpleLoadingTableViewSource(UITableView tableView, Type cellType, string cellIdentifier = null) : base(tableView, cellType, cellIdentifier)
        {
            tableView.RegisterNibForCellReuse(UINib.FromName(LoadingTableViewCell.Key, NSBundle.MainBundle), LoadingTableViewCell.Key);
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            UITableViewCell cell;
            if (ItemsSource != null && indexPath.Row == ItemsSource.Cast<object>().Count())
            {
                cell = tableView.DequeueReusableCell(LoadingTableViewCell.Key);
                (cell as LoadingTableViewCell).AnimateSpinner();
            }
            else
            {
                cell = base.GetCell(tableView, indexPath);
            }

            return cell;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            if (ItemsSource != null && !this.IsFullyLoaded)
            {
                // Add one row here for the loading-indicator.
                return base.RowsInSection(tableview, section) + 1;
            }
            else
            {
                return base.RowsInSection(tableview, section);
            }
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            if (ItemsSource == null || indexPath.Row < ItemsSource.Cast<object>().Count())
            {
                base.RowSelected(tableView, indexPath);
            }
        }

        public override void WillDisplay(UITableView tableView, UITableViewCell cell, NSIndexPath indexPath)
        {
            if (!IsFullyLoaded && LoadMoreCommand != null && indexPath.Row == ItemsSource.Cast<object>().Count() - 1)
            {
                LoadMoreCommand.Execute();
            }
        }
    }
}