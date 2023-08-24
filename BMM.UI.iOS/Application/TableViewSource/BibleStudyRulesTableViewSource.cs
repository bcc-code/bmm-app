using BMM.Core.Models.POs.BibleStudy.Interfaces;
using BMM.Core.Models.POs.Other.Interfaces;

namespace BMM.UI.iOS;

public class BibleStudyRulesTableViewSource : BaseTableViewSource
{
    public BibleStudyRulesTableViewSource(UITableView tableView) : base(tableView)
    {
    }

    protected override IEnumerable<ITableCellType> GetTableCellTypes()
    {
        return new[]
        {
            new TableCellType(typeof(IBibleStudyRulesHeaderPO), BibleStudyRulesHeaderTableViewCell.Key),
            new TableCellType(typeof(IBibleStudyRulesContentPO), BibleStudyRulesContentTableViewCell.Key)
        };
    }
}