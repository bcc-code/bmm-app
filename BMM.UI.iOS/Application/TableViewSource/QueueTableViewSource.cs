using BMM.UI.iOS.Constants;
using Foundation;
using UIKit;

namespace BMM.UI.iOS
{
    public class QueueTableViewSource : BaseSimpleTableViewSource
    {
        public QueueTableViewSource(UITableView tableView) :
            base(tableView, TrackTableViewCell.Key)
        { }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = base.GetCell(tableView, indexPath);

            UIView backgroundColorView = new UIView
            {
                BackgroundColor = AppColors.QueueBackgroundSelectedColor
            };
            cell.SelectedBackgroundView = backgroundColorView;
            cell.BackgroundColor = AppColors.QueueBackgroundColor;

            return cell;
        }
    }
}