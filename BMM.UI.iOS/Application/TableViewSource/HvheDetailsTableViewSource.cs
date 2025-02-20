using BMM.Core.Models.POs.BibleStudy;
using BMM.UI.iOS.CustomViews;

namespace BMM.UI.iOS;

public class HvheDetailsTableViewSource : BaseTableViewSource
{
    private readonly Action<bool> _scrolledToChurchesSelectorAction;

    public HvheDetailsTableViewSource(
        UITableView tableView,
        Action<bool> scrolledToChurchesSelectorAction) : base(tableView)
    {
        _scrolledToChurchesSelectorAction = scrolledToChurchesSelectorAction;
    }

    protected override UITableViewCell GetOrCreateCellFor(UITableView tableView, NSIndexPath indexPath, object item)
    {
        if (item is HvheChurchPO hvheChurchPO)
        {
            return tableView.DequeueReusableCell(hvheChurchPO.Church.IsHighlighted
                ? HighlightedChurchTableViewCell.Key
                : StandardChurchTableViewCell.Key);
        }
        
        return base.GetOrCreateCellFor(tableView, indexPath, item);
    }
    
    protected override IEnumerable<ITableCellType> GetTableCellTypes()
    {
        return
        [
            new TableCellType(typeof(HvheBoysVsGirlsPO), HvheBoysVsGirlsTableViewCell.Key),
            new TableCellType(typeof(HvheChurchesSelectorPO), HvheChurchesSelectorTableViewCell.Key),
            new TableCellType(typeof(HvheHeaderPO), HvheHeaderTableViewCell.Key),
            new TableCellType(typeof(HvheChurchPO), HighlightedChurchTableViewCell.Key),
            new TableCellType(typeof(HvheChurchPO), StandardChurchTableViewCell.Key)
        ];
    }

    public override void Scrolled(UIScrollView scrollView)
    {
        var visibleCells = TableView.VisibleCells;
        if (visibleCells.Length == 0)
            return;

        var cell = TableView.VisibleCells.FirstOrDefault(v => v is HvheChurchesSelectorTableViewCell);

        if (cell == null)
            return;
       
        var indexPath = TableView.IndexPathForCell(cell);
        if (indexPath == null)
            return;
        
        var cellFrame = TableView.RectForRowAtIndexPath(indexPath);
        var cellTop = TableView.ConvertRectToView(cellFrame, TableView.Superview).Y;
        var tableViewTop = TableView.Frame.Y;

        _scrolledToChurchesSelectorAction.Invoke(cellTop <= tableViewTop);
    }
}