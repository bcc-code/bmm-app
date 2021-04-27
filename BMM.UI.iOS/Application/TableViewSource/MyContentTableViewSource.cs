using System.Collections.Generic;
using BMM.Core.Models;
using UIKit;

namespace BMM.UI.iOS
{
    public class MyContentTableViewSource: VariableRowHeightTableViewSource
    {
        public MyContentTableViewSource(UITableView tableView) : base(tableView)
        {
        }

        protected override IEnumerable<IHeightAwareTableCellType> GetHeightAwareTableCellTypes()
        {
            return new List<IHeightAwareTableCellType>
            {
                new HeightAwareTableCellType(typeof(ListItem), TextListItemTableViewCell.Key, 60)
            };
        }
    }
}
