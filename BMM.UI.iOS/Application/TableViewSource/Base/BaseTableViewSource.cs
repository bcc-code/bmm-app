using BMM.UI.iOS.TableViewCell.Base;
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
            tableView.DeselectRow(indexPath, true);
            base.RowSelected(tableView, indexPath);
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