using Foundation;
using MvvmCross.ViewModels;
using System;
using System.Globalization;
using BMM.Core.ViewModels;
using MvvmCross.Binding.Extensions;
using MvvmCross.Commands;
using UIKit;

namespace BMM.UI.iOS
{
    public class LanguageEditableTableViewSource : BaseTableViewSource
    {
        // A fixed height for each cell. Done to speed up rendering of the single cells.
        public const int CellHeight = 55;

        public IMvxCommand LoadMoreCommand { get; set; }

        private bool _isFullyLoaded = true;

        public bool IsFullyLoaded
        {
            get => _isFullyLoaded;
            set
            {
                _isFullyLoaded = value;
                InvokeOnMainThread(delegate
                {
                    TableView.ReloadData();
                });
            }
        }

        public LanguageEditableTableViewSource(UITableView tableView)
            : base(tableView)
        {
            string[] nibNames = {LoadingTableViewCell.Key, LanguageContentTableViewCell.Key};

            tableView.RowHeight = CellHeight;

            foreach (string nibName in nibNames)
            {
                tableView.RegisterNibForCellReuse(UINib.FromName(nibName, NSBundle.MainBundle), nibName);
            }
        }

        protected override UITableViewCell GetOrCreateCellFor(UITableView tableView, NSIndexPath indexPath, object item)
        {
            return tableView.DequeueReusableCell(LanguageContentTableViewCell.Key);
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            MvxObservableCollection<CultureInfo> data = ItemsSource as MvxObservableCollection<CultureInfo>;

            UITableViewCell cell;
            if (data != null && indexPath.Row == data.Count)
            {
                cell = tableView.DequeueReusableCell(LoadingTableViewCell.Key);
                (cell as LoadingTableViewCell).AnimateSpinner();
            }
            else
            {
                cell = base.GetCell(tableView, indexPath);

                UIView backgroundColorView = new UIView();
                backgroundColorView.BackgroundColor = UIColor.FromRGB(239, 242, 245);
                cell.SelectedBackgroundView = backgroundColorView;

                ((LanguageContentTableViewCell)cell).Index = indexPath.Row + 1;
            }

            return cell;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            if (ItemsSource is MvxObservableCollection<CultureInfo> data && !IsFullyLoaded)
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
            MvxObservableCollection<CultureInfo> data = ItemsSource as MvxObservableCollection<CultureInfo>;

            if (data == null || indexPath.Row < data.Count)
            {
                base.RowSelected(tableView, indexPath);
            }
        }

        public override void WillDisplay(UITableView tableView, UITableViewCell cell, NSIndexPath indexPath)
        {
            base.WillDisplay(tableView, cell, indexPath);
            MvxObservableCollection<CultureInfo> data = ItemsSource as MvxObservableCollection<CultureInfo>;

            if (indexPath.Row == data.Count - 1)
            {
                LoadMoreCommand?.Execute();
            }
        }

        /// <summary>
        /// On Android <see cref="LanguageContentViewModel.DeleteAction"/> we show a message if the user tries to delete the last item. On iOS we prevent deleting it altogether.
        /// </summary>
        public override bool CanEditRow(UITableView tableView, NSIndexPath indexPath)
        {
            return ItemsSource.Count() > 1;
        }

        public override bool CanMoveRow(UITableView tableView, NSIndexPath indexPath)
        {
            return true;
        }

        public override UITableViewCellEditingStyle EditingStyleForRow(UITableView tableView, NSIndexPath indexPath)
        {
            return UITableViewCellEditingStyle.Delete;
        }

        public override void MoveRow(UITableView tableView, NSIndexPath sourceIndexPath, NSIndexPath destinationIndexPath)
        {
            if (sourceIndexPath.Row == destinationIndexPath.Row)
                return;

            MvxObservableCollection<CultureInfo> items = ItemsSource as MvxObservableCollection<CultureInfo>;
            if (items == null)
            {
                // Can not move rows in a collection that does not implement IMvxMultiListView
                return;
            }

            items.Move(sourceIndexPath.Row, destinationIndexPath.Row);
        }

        public override void CommitEditingStyle(UITableView tableView, UITableViewCellEditingStyle editingStyle, NSIndexPath indexPath)
        {
            MvxObservableCollection<CultureInfo> tableItems = ItemsSource as MvxObservableCollection<CultureInfo>;

            switch (editingStyle)
            {
                case UITableViewCellEditingStyle.Delete:
                    tableItems.RemoveAt(indexPath.Row);
                    break;

                case UITableViewCellEditingStyle.None:
                    break;
            }
        }
    }
}