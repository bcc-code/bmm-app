using BMM.Core.Models.POs.BibleStudy;

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
            new TableCellType(typeof(BibleStudyHeaderTableViewCell), BibleStudyHeaderTableViewCell.Key),
            new TableCellType(typeof(BibleStudyProgressTableViewCell), BibleStudyProgressTableViewCell.Key)
        };
    }

    protected override UITableViewCell GetOrCreateCellFor(UITableView tableView, NSIndexPath indexPath, object item)
    {
        string nibName = item switch
        {
            BibleStudyHeaderPO => BibleStudyHeaderTableViewCell.Key,
            BibleStudyProgressPO => BibleStudyProgressTableViewCell.Key,
            _ => null
        };

        return tableView.DequeueReusableCell(nibName);
    }
}