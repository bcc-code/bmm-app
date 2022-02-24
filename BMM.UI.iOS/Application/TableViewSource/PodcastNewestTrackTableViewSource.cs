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

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = base.GetCell(tableView, indexPath);

            TableView.BeginInvokeOnMainThread(() =>
            {
                var backgroundColorView = new UIView
                {
                    BackgroundColor = AppColors.NewestTrackBackgroundColor
                };
                cell.SelectedBackgroundView = backgroundColorView;
                cell.BackgroundColor = UIColor.Clear;
            });

            return cell;
        }
    }
}