using System.Collections.Generic;
using BMM.Core.Models;
using BMM.Core.Models.POs.Other.Interfaces;
using UIKit;

namespace BMM.UI.iOS
{
    public class TrackInfoTableViewSource : BaseTableViewSource
    {
        public TrackInfoTableViewSource(UITableView tableView) : base(tableView)
        {
        }

        protected override IEnumerable<ITableCellType> GetTableCellTypes()
        {
            return new[]
            {
                new TableCellType(typeof(IExternalRelationListItemPO), ExternalRelationListItemTableViewCell.Key),
                new TableCellType(typeof(ISelectableListContentItemPO), TextListItemTableViewCell.Key),
                new TableCellType(typeof(ISectionHeaderPO), SectionHeaderTableViewCell.Key)
            };
        }
    }
}