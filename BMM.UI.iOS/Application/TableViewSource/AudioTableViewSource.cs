using Foundation;
using UIKit;

namespace BMM.UI.iOS
{
    public class AudioTableViewSource : SimpleLoadingTableViewSource
    {
        public AudioTableViewSource(UITableView tableView, string nibName, string cellIdentifier = null, NSBundle bundle = null) : base(tableView, nibName, cellIdentifier, bundle)
        {
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            UITableViewCell cell = base.GetCell(tableView, indexPath);

            UIView backgroundColorView = new UIView();
            backgroundColorView.BackgroundColor = UIColor.White;
            cell.SelectedBackgroundView = backgroundColorView;

            return cell;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            tableView.DeselectRow(indexPath, true);
            base.RowSelected(tableView, indexPath);
        }
    }
}