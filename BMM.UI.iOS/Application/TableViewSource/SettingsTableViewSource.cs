using System;
using System.Collections.Generic;
using BMM.Core.Models;
using BMM.Core.Models.POs.Other;
using Foundation;
using UIKit;

namespace BMM.UI.iOS
{
    public class SettingsTableViewSource : BaseTableViewSource
    {
        public SettingsTableViewSource(UITableView tableView)
            : base(tableView)
        {
            tableView.RegisterNibForCellReuse(UINib.FromName(ProfileListItemTableViewCell.Key, NSBundle.MainBundle), ProfileListItemTableViewCell.Key);
            tableView.RegisterNibForCellReuse(UINib.FromName(CheckboxListItemTableViewCell.Key, NSBundle.MainBundle), CheckboxListItemTableViewCell.Key);
            tableView.RegisterNibForCellReuse(UINib.FromName(TextListItemDetailTableViewCell.Key, NSBundle.MainBundle), TextListItemDetailTableViewCell.Key);
            tableView.RegisterNibForCellReuse(UINib.FromName(SectionHeaderTableViewCell.Key, NSBundle.MainBundle), SectionHeaderTableViewCell.Key);
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = base.GetCell(tableView, indexPath);

            if (cell is SectionHeaderTableViewCell)
                cell.SelectionStyle = UITableViewCellSelectionStyle.None;

            cell.SelectedBackgroundView = new UIView
            {
                BackgroundColor = UIColor.FromRGB(239, 242, 245)
            };

            return cell;
        }
        
        protected override IEnumerable<ITableCellType> GetTableCellTypes()
        {
            return new[]
            {
                new TableCellType(typeof(ProfileListItem), ProfileListItemTableViewCell.Key),
                new TableCellType(typeof(CheckboxListItemPO), CheckboxListItemTableViewCell.Key),
                new TableCellType(typeof(SelectableListItem), TextListItemDetailTableViewCell.Key),
                new TableCellType(typeof(SectionHeaderPO), SectionHeaderTableViewCell.Key),
            };
        }
    }
}