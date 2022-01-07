using BMM.Core.Models.POs;
using Foundation;
using MvvmCross.Platforms.Ios.Binding.Views;
using UIKit;

namespace BMM.UI.iOS
{
    public class ChangeTrackLanguageTableViewSource : MvxTableViewSource
    {
        public ChangeTrackLanguageTableViewSource(UITableView tableView) : base(tableView)
        {
            tableView.Source = this;
            tableView.RegisterNibForCellReuse(StandardHeaderTableViewCell.Nib, StandardHeaderTableViewCell.Key);
            tableView.RegisterNibForCellReuse(StandardSelectableTableViewCell.Nib, StandardSelectableTableViewCell.Key);
            tableView.RowHeight = UITableView.AutomaticDimension;
            tableView.EstimatedRowHeight = 50;
        }

        protected override UITableViewCell GetOrCreateCellFor(UITableView tableView, NSIndexPath indexPath, object item)
        {
            string key = item switch
            {
                HeaderPO _ => StandardHeaderTableViewCell.Key,
                StandardSelectablePO _ => StandardSelectableTableViewCell.Key,
                _ => null
            };

            return tableView.DequeueReusableCell(key, indexPath);
        }
    }
}