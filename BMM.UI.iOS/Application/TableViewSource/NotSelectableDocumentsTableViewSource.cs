using BMM.Core.Models.POs.ListeningStreaks;
using BMM.Core.Models.POs.Other;

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