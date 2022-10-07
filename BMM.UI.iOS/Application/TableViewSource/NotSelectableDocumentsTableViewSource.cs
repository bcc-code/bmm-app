using BMM.Core.Models.POs.ListeningStreakPO;
using BMM.Core.Models.POs.Other;
using Foundation;
using UIKit;

namespace BMM.UI.iOS
{
    public class NotSelectableDocumentsTableViewSource : DocumentsTableViewSource
    {
        public NotSelectableDocumentsTableViewSource(UITableView tableView) : base(tableView)
        {
        }

        protected override UITableViewCell GetOrCreateCellFor(UITableView tableView, NSIndexPath indexPath, object item)
        {
            var cell = base.GetOrCreateCellFor(tableView, indexPath, item);
            if (item == null)
                return cell;

            switch (item)
            {
                case ChapterHeaderPO:
                case DiscoverSectionHeaderPO:
                case ListeningStreakPO:
                    cell.SelectionStyle = UITableViewCellSelectionStyle.None;
                    break;
            }

            return cell;
        }
    }
}