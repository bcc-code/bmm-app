using UIKit;

namespace BMM.UI.iOS.Extensions
{
    public static class UiTableExtensions
    {
        // shamelessly copied form: https://useyourloaf.com/blog/variable-height-table-view-header/
        public static void ResizeHeaderView(this UITableView tableView)
        {
            if (tableView.TableHeaderView == null)
                return;

            var headerView = tableView.TableHeaderView;

            var size = headerView.SystemLayoutSizeFittingSize(UIView.UILayoutFittingCompressedSize);

            if (headerView.Frame.Size.Height == size.Height)
                return;

            var frame = headerView.Frame;
            var frameSize = frame.Size;
            frameSize.Height = size.Height;

            frame.Size = frameSize;
            headerView.Frame = frame;

            tableView.TableHeaderView = headerView;
            tableView.LayoutIfNeeded();
        }
    }
}