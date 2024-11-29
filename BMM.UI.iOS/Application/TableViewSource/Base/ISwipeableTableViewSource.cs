namespace BMM.UI.iOS.TableViewSource.Base
{
    public interface ISwipeableTableViewSource
    {
        void ResetVisibleCells(bool animate = false);
        UITableViewCell CellWithSwipeInProgress { get; set; }
    }
}