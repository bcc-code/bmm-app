using BMM.UI.iOS.Constants;
using Foundation;
using UIKit;

namespace BMM.UI.iOS
{
    public class PodcastNewestTrackTableViewSource : BaseSimpleTableViewSource
    {
        public PodcastNewestTrackTableViewSource(UITableView tableView) :
            base(tableView, TrackTableViewCell.Key)
        { }

        protected override UITableViewCell GetOrCreateCellFor(UITableView tableView, NSIndexPath indexPath, object item)
        {
            var cell = base.GetOrCreateCellFor(tableView, indexPath, item);

            UIView backgroundColorView = new UIView
            {
                BackgroundColor = AppColors.NewestTrackBackgroundColor
            };
            cell.SelectedBackgroundView = backgroundColorView;
            cell.BackgroundColor = UIColor.Clear;

            return cell;
        }
    }
}