using BMM.Core.Models.POs.BibleStudy.Interfaces;
using BMM.Core.Models.POs.Other.Interfaces;

namespace BMM.UI.iOS;

public class BibleStudyTableViewSource : BaseTableViewSource
{
    public BibleStudyTableViewSource(UITableView tableView) : base(tableView)
    {
    }

    protected override IEnumerable<ITableCellType> GetTableCellTypes()
    {
        return new[]
        {
            new TableCellType(typeof(IBibleStudyHeaderPO), BibleStudyHeaderTableViewCell.Key),
            new TableCellType(typeof(IBibleStudyProgressPO), BibleStudyProgressTableViewCell.Key),
            new TableCellType(typeof(IExternalRelationListItemPO), ExternalRelationListItemTableViewCell.Key),
            new TableCellType(typeof(ISelectableListContentItemPO), TextListItemTableViewCell.Key),
            new TableCellType(typeof(ISectionHeaderPO), SectionHeaderTableViewCell.Key)
        };
    }
}