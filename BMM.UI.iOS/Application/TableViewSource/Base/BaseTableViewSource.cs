using BMM.Api.Framework;
using BMM.Core.Exceptions;
using BMM.Core.Implementations.Analytics;
using BMM.UI.iOS.TableViewCell.Base;
using MvvmCross;
using MvvmCross.Binding.Extensions;
using MvvmCross.Platforms.Ios.Binding.Views;

namespace BMM.UI.iOS
{
    public class BaseTableViewSource : MvxTableViewSource
    {
        protected virtual IEnumerable<ITableCellType> GetTableCellTypes()
        {
            return new List<ITableCellType>();
        }

        private readonly IEnumerable<ITableCellType> _tableCellTypes;

        protected BaseTableViewSource(UITableView tableView)
            : base(tableView)
        {
            tableView.Source = this;

            _tableCellTypes = GetTableCellTypes();

            foreach (var cellType in _tableCellTypes)
            {
                var cellUibName = cellType.UibName;
                tableView.RegisterNibForCellReuse(UINib.FromName(cellUibName, NSBundle.MainBundle), cellUibName);
            }
        }
        
        public event EventHandler ScrolledEvent;

        public override void ReloadTableData()
        {
            UIView.PerformWithoutAnimation(() =>
            {
                TableView.ReloadData();
                TableView.BeginUpdates();
                TableView.EndUpdates();
            });
        }

        protected override UITableViewCell GetOrCreateCellFor(UITableView tableView, NSIndexPath indexPath, object item)
        {
            foreach (var cellInfo in _tableCellTypes)
            {
                if (cellInfo.ItemType.IsInstanceOfType(item))
                {
                    return tableView.DequeueReusableCell(cellInfo.UibName);
                }
            }

            return null;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            try
            {
                tableView.DeselectRow(indexPath, true);
                base.RowSelected(tableView, indexPath);
            }
            catch (Exception e) when (e is not ForcedException)
            {
                Mvx
                    .IoCProvider
                    .Resolve<IAnalytics>()
                    .LogEvent(
                        $"{nameof(RowSelected)} error " +
                        $"Type: {GetType()}",
                        new Dictionary<string, object>()
                        {
                            {nameof(indexPath), indexPath.Row},
                            {"item count", ItemsSource?.Count().ToString()}
                        });
            }
        }
        
        public override void WillDisplay(UITableView tableView, UITableViewCell cell, NSIndexPath indexPath)
        {
            if (cell is IEventsHoldingTableViewCell eventsHoldingViewCell)
                eventsHoldingViewCell.AttachEvents();
        }
        
        public override void CellDisplayingEnded(UITableView tableView, UITableViewCell cell, NSIndexPath indexPath)
        {
            if (cell is IEventsHoldingTableViewCell eventsHoldingViewCell)
                eventsHoldingViewCell.DetachEvents();
        }
        
        public override void Scrolled(UIScrollView scrollView)
        {
            ScrolledEvent?.Invoke(this, EventArgs.Empty);
        }
    }
}