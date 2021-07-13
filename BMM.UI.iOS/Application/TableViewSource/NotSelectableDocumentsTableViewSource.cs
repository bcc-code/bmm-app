using System.Reflection;
using BMM.Api.Implementation.Models;
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
            PropertyInfo documentProperty = item.GetType().GetProperty("Item");
            Document document = documentProperty?.GetValue(item) as Document;

            var cell = base.GetOrCreateCellFor(tableView, indexPath, item);
            if (document == null)
                return cell;

            switch (document.DocumentType)
            {
                case DocumentType.ChapterHeader:
                case DocumentType.DiscoverSectionHeader:
                case DocumentType.ListeningStreak:
                    cell.SelectionStyle = UITableViewCellSelectionStyle.None;
                    break;
            }

            return cell;
        }
    }
}