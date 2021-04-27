using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using UIKit;

namespace BMM.UI.iOS
{
    public abstract class VariableRowHeightTableViewSource: BaseTableViewSource
    {
        protected override IEnumerable<ITableCellType> GetTableCellTypes()
        {
            return GetHeightAwareTableCellTypes();
        }

        protected virtual IEnumerable<IHeightAwareTableCellType> GetHeightAwareTableCellTypes()
        {
            return new List<IHeightAwareTableCellType>();
        }

        private readonly IEnumerable<IHeightAwareTableCellType> _heightAwareTableCellTypes;

        protected VariableRowHeightTableViewSource(UITableView tableView) : base(tableView)
        {
            _heightAwareTableCellTypes = GetHeightAwareTableCellTypes();
        }

        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            var itemType = GetItemAt(indexPath).GetType();

            return _heightAwareTableCellTypes
                .Where(cellType => cellType.ItemType == itemType)
                .Select(cellType => cellType.Height)
                .First();
        }

        public override nfloat EstimatedHeight(UITableView tableView, NSIndexPath indexPath)
        {
            return GetHeightForRow(tableView, indexPath);
        }
    }
}