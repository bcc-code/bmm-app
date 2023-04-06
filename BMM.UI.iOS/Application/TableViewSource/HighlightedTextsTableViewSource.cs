using BMM.Core.Models.POs.Tracks;

namespace BMM.UI.iOS;

public class HighlightedTextsTableViewSource : BaseTableViewSource
{
    public HighlightedTextsTableViewSource(UITableView tableView) : base(tableView)
    {
    }

    protected override IEnumerable<ITableCellType> GetTableCellTypes()
    {
        return new[]
        {
            new TableCellType(typeof(HighlightedTextTableViewCell), HighlightedTextTableViewCell.Key),
            new TableCellType(typeof(HighlightedTextHeaderTableViewCell), HighlightedTextHeaderTableViewCell.Key)
        };
    }

    protected override UITableViewCell GetOrCreateCellFor(UITableView tableView, NSIndexPath indexPath, object item)
    {
        string nibName = item switch
        {
            HighlightedTextHeaderPO => HighlightedTextHeaderTableViewCell.Key,
            HighlightedTextPO => HighlightedTextTableViewCell.Key,
            _ => null
        };

        return tableView.DequeueReusableCell(nibName);
    }
}