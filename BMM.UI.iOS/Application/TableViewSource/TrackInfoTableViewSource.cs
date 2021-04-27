using System.Collections.Generic;
using BMM.Core.Models;
using UIKit;

namespace BMM.UI.iOS
{
    public class TrackInfoTableViewSource: SectionedTableViewSource
    {
        public TrackInfoTableViewSource(UITableView tableView) : base(tableView)
        {
        }

        protected override IEnumerable<IHeightAwareTableCellType> GetHeightAwareTableCellTypes()
        {
            return new List<IHeightAwareTableCellType>
            {
                new HeightAwareTableCellType(typeof(ExternalRelationListItem), ExternalRelationListItemTableViewCell.Key, 60),
                new HeightAwareTableCellType(typeof(SelectableListItem), TextListItemTableViewCell.Key, 65),
            };
        }
    }
}